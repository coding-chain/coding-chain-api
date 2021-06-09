using System;
using System.Collections.Generic;
using Application.Read.Plagiarism;
using Domain.ProgrammingLanguages;

namespace Application.Contracts.Dtos
{
    public record RunParticipationTestsDto(Guid Id, LanguageEnum Language, string? HeaderCode,
        IList<RunParticipationTestsDto.Test> Tests,
        IList<RunParticipationTestsDto.Function> Functions)
    {
        public record Test(Guid Id, string Name, string OutputValidator, string InputGenerator);

        public record Function(Guid Id, string Code, int Order);
    }
    

    public record PlagiarismAnalyzeExecutionDto
        (Function SuspectedFunction, IList<Function> ComparedFunctions);
    
    public record PrepareParticipationExecutionDto(Guid Id, LanguageEnum Language);
    public record CleanParticipationExecutionDto(Guid Id, LanguageEnum Language);

}