using System.Diagnostics;
using Application.Contracts.IService;

namespace CodingChainApi.Infrastructure.Services
{
    public class ProcessService : IProcessService
    {
        public void RunProcess(string processName, string arguments, DataReceivedEventHandler? outputHandler,
            DataReceivedEventHandler? errorHandler)
        {
            using var process = new Process
            {
                StartInfo =
                {
                    FileName = processName,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    Arguments = arguments
                }
            };

            process.ErrorDataReceived += errorHandler;
            process.OutputDataReceived += outputHandler;
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
        }
    }
}