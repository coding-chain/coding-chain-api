using System;
using System.Diagnostics;

namespace Application.Sut
{
    public class RunSut
    {
        private readonly ISutRepository _sutRepository;

        public RunSut(ISutRepository sutRepository)
        {
            _sutRepository = sutRepository;
        }

        public void Execute(string content, string language, DataReceivedEventHandler onData,
            DataReceivedEventHandler onError)
        {
            var path = _sutRepository.GetArgumentsByLanguage(language);
            var runtime = _sutRepository.GetRuntimeByLanguage(language);

            using var process = new Process
            {
                StartInfo =
                {
                    FileName = runtime,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    Arguments = path
                }
            };
            process.OutputDataReceived += (d, e) => Console.WriteLine(e.Data); 
            // process.OutputDataReceived += onData;
            process.ErrorDataReceived += onError;
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
        }
    }
}