using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Write.Contracts;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Domain.Participations;
using Domain.StepEditions;
using Domain.Teams;
using Domain.Tournaments;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using StepEntity = Domain.Participations.StepEntity;

namespace CodingChainApi.Infrastructure.Repositories.AggregateRepositories
{
    public class ParticipationRepository : IParticipationRepository
    {
        private readonly CodingChainContext _context;

        public ParticipationRepository(CodingChainContext context)
        {
            _context = context;
        }

        public async Task<ParticipationId> SetAsync(ParticipationAggregate aggregate)
        {
            var participation = await ToModel(aggregate);
            _context.Participations.Upsert(participation);
            await _context.SaveChangesAsync();
            return new ParticipationId(participation.Id);
        }

        public async Task<ParticipationAggregate?> FindByIdAsync(ParticipationId id)
        {
            var participation = await _context.Participations
                .Include(p => p.Step)
                .ThenInclude(p => p.TournamentSteps)
                .Include(p => p.Tournament)
                .Include(p => p.Functions)
                .ThenInclude<Participation, Function, IList<UserFunction>>(f => f.UserFunctions)
                .Include(p => p.Team)
                .ThenInclude(t => t.UserTeams)
                .FirstOrDefaultAsync(p =>
                    !p.Tournament.IsDeleted && !p.Step.IsDeleted && !p.Team.IsDeleted && p.Id == id.Value);
            return participation is null ? null : ToAggregate(participation);
        }


        public async Task RemoveAsync(ParticipationId id)
        {
            var participation = await _context.Participations
                .FirstOrDefaultAsync(t => t.Id == id.Value);
            if (participation is not null)
                _context.Remove(participation);
            await _context.SaveChangesAsync();
        }

        public Task<ParticipationId> NextIdAsync()
        {
            return new ParticipationId(Guid.NewGuid()).ToTask();
        }

        private static ParticipationAggregate ToAggregate(Participation participation)
        {
            return ParticipationAggregate.Restore(
                new ParticipationId(participation.Id),
                ToTeamEntity(participation.Team),
                ToTournamentEntity(participation.Tournament),
                ToStepEntity(participation.Step),
                participation.StartDate,
                participation.EndDate,
                participation.CalculatedScore,
                participation.Functions.Where(f => !f.IsDeleted).Select(ToFunctionEntity).ToList()
            );
        }

        private static TeamEntity ToTeamEntity(Team team)
        {
            return new(
                new TeamId(team.Id),
                team.UserTeams.Select(t => new UserId(t.UserId)).ToList()
            );
        }

        private static TournamentEntity ToTournamentEntity(Tournament tournament)
        {
            return new(
                new TournamentId(tournament.Id), tournament.IsPublished);
        }

        private static StepEntity ToStepEntity(Step step)
        {
            return new(
                new StepId(step.Id),
                step.TournamentSteps.Select(tS => new TournamentId(tS.TournamentId)).ToList());
        }

        private static FunctionEntity ToFunctionEntity(Function function)
        {
            var lastUserFunction = function.UserFunctions
                .OrderByDescending(f => f.LastModificationDate)
                .First();
            return new FunctionEntity(
                new FunctionId(function.Id),
                new UserId(lastUserFunction.UserId),
                function.Code,
                lastUserFunction.LastModificationDate,
                function.Order
            );
        }

        private async Task<Participation> ToModel(ParticipationAggregate aggregate)
        {
            var participation = await GetModel(aggregate.Id.Value);
            SynchronizeFunctions(aggregate, participation);
            participation.Step = _context.Steps.First(s => !s.IsDeleted && s.Id == aggregate.StepEntity.Id.Value);
            participation.Tournament =
                _context.Tournaments.First(t => !t.IsDeleted && t.Id == aggregate.TournamentEntity.Id.Value);
            participation.Team = _context.Teams.First(t => !t.IsDeleted && t.Id == aggregate.Team.Id.Value);
            participation.CalculatedScore = aggregate.CalculatedScore;
            participation.EndDate = aggregate.EndDate;
            participation.StartDate = aggregate.StartDate;
            return participation;
        }

        private void SynchronizeFunctions(ParticipationAggregate aggregate, Participation participation)
        {
            var removedFunctions = GetRemovedFunctions(aggregate, participation);
            removedFunctions.ForEach(function => function.IsDeleted = true);
            var currentFunctions = GetCurrentFunctions(participation);
            var newFunctions = GetNewFunctions(aggregate, currentFunctions);
            currentFunctions.ForEach(currentFunction =>
            {
                var function = aggregate.Functions.First(f => f.Id.Value == currentFunction.Id);
                currentFunction.Code = function.Code;
                currentFunction.Order = function.Order;
                var userFunction =
                    currentFunction.UserFunctions.FirstOrDefault(uF => uF.UserId == function.UserId.Value);
                if (userFunction is null)
                    currentFunction.UserFunctions.Add(new UserFunction
                        {UserId = function.UserId.Value, LastModificationDate = function.LastModificationDate});
                else
                    userFunction.LastModificationDate = function.LastModificationDate;
            });
            newFunctions.ForEach(function => participation.Functions.Add(function));
        }

        private async Task<Participation> GetModel(Guid participationId)
        {
            return await _context.Participations
                       .Include(p => p.Functions)
                       .ThenInclude<Participation, Function, IList<UserFunction>>(f => f.UserFunctions)
                       // .Include(p => p.Step)
                       // .ThenInclude(s => s.TournamentSteps)
                       // .Include(p => p.Tournament)
                       // .Include(p => p.Team)
                       // .ThenInclude(p => p.UserTeams)
                       .FirstOrDefaultAsync(p =>
                           !p.Step.IsDeleted && !p.Tournament.IsDeleted && !p.Team.IsDeleted && p.Id == participationId)
                   ?? new Participation {Id = participationId};
        }

        private List<Function> GetNewFunctions(ParticipationAggregate aggregate, List<Function> currentFunctions)
        {
            return aggregate.Functions
                .Where(function => currentFunctions.All(f => f.Id != function.Id.Value))
                .Select(function => new Function
                {
                    Id = function.Id.Value, Code = function.Code, Order = function.Order,
                    UserFunctions = new List<UserFunction>
                    {
                        new()
                        {
                            FunctionId = function.Id.Value, UserId = function.UserId.Value,
                            LastModificationDate = function.LastModificationDate
                        }
                    }
                })
                .ToList();
        }

        private static List<Function> GetCurrentFunctions(Participation participation)
        {
            return participation.Functions
                .Where(function => !function.IsDeleted)
                .ToList();
        }

        private static List<Function> GetRemovedFunctions(ParticipationAggregate aggregate, Participation participation)
        {
            return participation.Functions
                .Where(function => aggregate.Functions.All(f => f.Id.Value != function.Id))
                .ToList();
        }
    }
}