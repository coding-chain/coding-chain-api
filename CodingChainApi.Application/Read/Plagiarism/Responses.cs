using System;
using System.Collections;
using System.Collections.Generic;

namespace Application.Read.Plagiarism
{
    public record FunctionCodeNavigation(Guid Id, Guid LastEditorId, Guid LanguageId, DateTime LastModificationDate,
        string Code);
    
    public record PlagiarizedFunctionNavigation(Guid Id, Guid LastEditorId, Guid ParticipationId, Guid LanguageId, DateTime LastModificationDate,
        string Code, double Rate, DateTime DetectionDate);

    public record SuspectFunctionNavigation(Guid Id, Guid LastEditorId, Guid ParticipationId,  Guid LanguageId, DateTime LastModificationDate,
        string Code, IList<Guid> PlagiarizedFunctionsIds);
    
}