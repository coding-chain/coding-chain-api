using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Write.Contracts;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Domain.StepEditions;
using Domain.Tournaments;
using Microsoft.EntityFrameworkCore;

namespace CodingChainApi.Infrastructure.Repositories.AggregateRepositories
{
    public class TournamentRepository : ITournamentRepository
    {
        private readonly CodingChainContext _context;

        public TournamentRepository(CodingChainContext context)
        {
            _context = context;
        }

        public async Task<TournamentId> SetAsync(TournamentAggregate aggregate)
        {
            var tournament = await ToModel(aggregate);
            _context.Tournaments.Upsert(tournament);
            await _context.SaveChangesAsync();
            return new TournamentId(tournament.Id);
        }

        public async Task<TournamentAggregate?> FindByIdAsync(TournamentId id)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.TournamentSteps)
                .ThenInclude<Tournament, TournamentStep, Step>(t => t.Step)
                .FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == id.Value);
            return tournament is null ? null : ToAggregate(tournament);
        }

        public async Task RemoveAsync(TournamentId id)
        {
            var tournament = await _context.Tournaments
                .FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == id.Value);
            if (tournament is not null)
                tournament.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public Task<TournamentId> NextIdAsync()
        {
            return new TournamentId(Guid.NewGuid()).ToTask();
        }

        private TournamentAggregate ToAggregate(Tournament tournamentEntity)
        {
            return TournamentAggregate.Restore(
                new TournamentId(tournamentEntity.Id),
                tournamentEntity.Name,
                tournamentEntity.Description,
                tournamentEntity.IsPublished,
                tournamentEntity.StartDate,
                tournamentEntity.EndDate,
                tournamentEntity.TournamentSteps
                    .Where(tS => !tS.Step.IsDeleted)
                    .Select(tS => new StepEntity(
                        new StepId(tS.StepId),
                        tS.Order,
                        tS.IsOptional
                    ))
                    .ToList()
            );
        }

        private async Task<Tournament> ToModel(TournamentAggregate aggregate)
        {
            var tournament = await GetTournament(aggregate.Id.Value);
            var removedSteps = GetRemovedSteps(aggregate, tournament);
            var currentSteps = GetCurrentSteps(tournament)
                .Where(step => !removedSteps.Contains(step))
                .ToList();
            var newSteps = GetNewSteps(aggregate, currentSteps);
            currentSteps.ForEach(currentStep =>
            {
                var step = aggregate.Steps.First(m => m.Id.Value == currentStep.StepId);
                currentStep.Order = step.Order;
                currentStep.IsOptional = step.IsOptional;
            });
            newSteps.ForEach(tS => tournament.TournamentSteps.Add(tS));
            _context.TournamentSteps.RemoveRange(removedSteps);
            tournament.Name = aggregate.Name;
            tournament.Description = aggregate.Description;
            tournament.EndDate = aggregate.EndDate;
            tournament.StartDate = aggregate.StartDate;
            tournament.IsPublished = aggregate.IsPublished;
            return tournament;
        }

        private static List<TournamentStep> GetNewSteps(TournamentAggregate aggregate,
            List<TournamentStep> currentSteps)
        {
            return aggregate.Steps
                .Where(s => currentSteps.All(tS => tS.StepId != s.Id.Value))
                .Select(s => new TournamentStep {Order = s.Order, IsOptional = s.IsOptional, StepId = s.Id.Value})
                .ToList();
        }

        private static List<TournamentStep> GetCurrentSteps(Tournament tournament)
        {
            return tournament.TournamentSteps
                .Where(tS => !tS.Step.IsDeleted)
                .ToList();
        }

        private static List<TournamentStep> GetRemovedSteps(TournamentAggregate aggregate, Tournament tournament)
        {
            return tournament.TournamentSteps
                .Where(tS => aggregate.Steps.All(s => s.Id.Value != tS.StepId))
                .ToList();
        }

        private async Task<Tournament> GetTournament(Guid aggregateId)
        {
            return await _context.Tournaments
                .Include(t => t.TournamentSteps)
                .FirstOrDefaultAsync(t => !t.IsDeleted && aggregateId == t.Id) ?? new Tournament {Id = aggregateId};
        }
    }
}