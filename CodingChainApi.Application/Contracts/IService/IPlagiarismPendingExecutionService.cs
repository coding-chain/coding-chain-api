using System;
using System.Collections.Generic;
using MediatR;

namespace Application.Contracts.IService
{
    public record Function(Guid Id, string Code);

    public record PlagiarismAnalyzeExecutionDto
        (Function SuspectedFunction, IList<Function> ComparedFunctions);

    public interface IPlagiarismPendingExecutionService
    {
        public void StartExecution(PlagiarismAnalyzeExecutionDto command);

    }
}