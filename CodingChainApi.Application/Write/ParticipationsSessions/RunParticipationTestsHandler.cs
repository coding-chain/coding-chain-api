using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Contracts.Dtos;
using Application.Contracts.IService;
using Application.Read.Contracts;
using Application.Write.Contracts;
using Domain.Participations;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Application.Write.ParticipationsSessions
{
    [Authorize]
    public record RunParticipationCommand(Guid ParticipationId) : IRequest<string>;

    public class RunParticipationTestsHandler : IRequestHandler<RunParticipationCommand, string>
    {
        private readonly IDispatcher<RunParticipationTestsDto> _participationExecutionService;
        private readonly IParticipationsSessionsRepository _participationsSessionsRepository;
        private readonly IReadProgrammingLanguageRepository _readProgrammingLanguageRepository;
        private readonly IReadStepRepository _readStepRepository;
        private readonly IReadTestRepository _readTestRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITimeService _timeService;
        public RunParticipationTestsHandler(IDispatcher<RunParticipationTestsDto> participationExecutionService,
            IParticipationsSessionsRepository participationsSessionsRepository,
            IReadProgrammingLanguageRepository readProgrammingLanguageRepository,
            IReadStepRepository readStepRepository, IReadTestRepository readTestRepository, ICurrentUserService currentUserService, ITimeService timeService)
        {
            _participationExecutionService = participationExecutionService;
            _participationsSessionsRepository = participationsSessionsRepository;
            _readProgrammingLanguageRepository = readProgrammingLanguageRepository;
            _readStepRepository = readStepRepository;
            _readTestRepository = readTestRepository;
            _currentUserService = currentUserService;
            _timeService = timeService;
        }

        public async Task<string> Handle(RunParticipationCommand request,
            CancellationToken cancellationToken)
        {
            var participation =
                await _participationsSessionsRepository.FindByIdAsync(new ParticipationId(request.ParticipationId));
            if (participation is null)
                throw new NotFoundException(request.ParticipationId.ToString(), "ParticipationSession");
            
            participation.ValidateProcessStart(_currentUserService.UserId);
            
            var step = await _readStepRepository.GetOneStepNavigationById(participation.StepEntity.Id.Value);
            if (step is null)
                throw new NotFoundException(participation.StepEntity.Id.Value.ToString(), "ParticipationSessionStep");
            
            var language = await _readProgrammingLanguageRepository.GetOneLanguageNavigationByIdAsync(step.LanguageId);
            if (language is null)
                throw new NotFoundException(step.LanguageId.ToString(), "ParticipationSessionStepLanguage");
            
            var tests = await _readTestRepository.GetAllTestNavigationByStepId(participation.StepEntity.Id.Value);
            var runTestDto = new RunParticipationTestsDto(
                participation.Id.Value,
                language.Name,
                step.HeaderCode ,
                tests.Select(t => new RunParticipationTestsDto.Test(t.Id, t.Name, t.OutputValidator, t.InputGenerator)).ToList(),
                participation.Functions
                    .Where(f => f.Order is not null)
                    .Select(f => new RunParticipationTestsDto.Function(f.Id.Value, f.Code, f.Order!.Value))
                    .ToList()
            );
            participation.StartProcess(_timeService.Now());
            _participationExecutionService.Dispatch(runTestDto);
            await _participationsSessionsRepository.SetAsync(participation);
            return await Task.FromResult(request.ParticipationId.ToString());
        }
    }
}