using System;
using System.Collections.Generic;
using Application.Contracts.Processes;
using Application.Write.ParticipationsSessions;
using Domain.ProgrammingLanguages;
using MediatR;

namespace Application.Contracts.IService
{
    public record RunParticipationTestsDto(Guid Id, LanguageEnum Language, string? HeaderCode,
        IList<RunParticipationTestsDto.Test> Tests,
        IList<RunParticipationTestsDto.Function> Functions)
    {
        public record Test(Guid Id, string Name, string OutputValidator, string InputGenerator);

        public record Function(Guid Id, string Code, int Order);
    }
    public interface IParticipationPendingExecutionService
    {
        public void StartExecution(RunParticipationTestsDto command);
    }
}