using System;
using Application.Contracts.Processes;
using Application.Write.Code.CodeExecution;

namespace Application.Contracts.IService
{
    public interface IParticipationExecutionService
    {
        public void StartExecution(RunParticipationTestsCommand command);

        public IProcessEndHandler? FollowExecution(Guid participationId);
    }
}