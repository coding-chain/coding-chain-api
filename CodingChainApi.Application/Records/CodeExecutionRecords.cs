using System;
using System.Collections.Generic;
using MediatR;

namespace NeosCodingApi.Records
{
    public interface CodeExecutionRecords
    {
        public record RunParticipationTestsCommand(Guid ParticipationId, string Language, string HeaderCode,
            IList<RunParticipationTestsCommand.Test> Tests,
            IList<RunParticipationTestsCommand.Function> Functions) : IRequest<string>
        {
            public record Test(string OutputValidator, string InputGenerator);

            public record Function(string Code, int Order);
        }
    }
}