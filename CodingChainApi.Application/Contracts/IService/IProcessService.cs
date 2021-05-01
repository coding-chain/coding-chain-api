using System.Diagnostics;

namespace Application.Contracts.IService
{
    public interface IProcessService
    {
        public void RunProcess(string processName, string arguments, DataReceivedEventHandler? outputHandler,
            DataReceivedEventHandler? errorHandler);
    }
}