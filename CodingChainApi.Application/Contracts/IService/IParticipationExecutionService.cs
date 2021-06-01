using System;
using System.Collections.Generic;
using Application.Contracts.Processes;
using Application.Write.ParticipationsSessions;
using MediatR;

namespace Application.Contracts.IService
{
    public record RunParticipationTestsDto(Guid ParticipationId, string Language, string? HeaderCode,
        IList<RunParticipationTestsDto.Test> Tests,
        IList<RunParticipationTestsDto.Function> Functions)
    {
        public record Test(string OutputValidator, string InputGenerator);

        public record Function(string Code, int Order);
    }
    public interface IParticipationExecutionService
    {
        public void StartExecution(RunParticipationTestsDto command);
    }
}