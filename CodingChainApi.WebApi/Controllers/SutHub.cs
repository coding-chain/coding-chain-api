using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace NeosCodingApi.Controllers
{
    public class SutHub : Hub

    {
        public async Task RunSut(CodeRequest request)

        {
            await File.WriteAllTextAsync("C:\\NEOS\\CodingChainApi\\templates\\csharp_template\\Sut.cs", request.Content);


            using var process = new Process
            {
                StartInfo =
                {
                    FileName = "dotnet",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    Arguments = "test C:\\NEOS\\CodingChainApi\\templates\\csharp_template"
                }
            };
            process.OutputDataReceived += (sender, data) => Clients.Caller.SendAsync("ReceiveOutput", data.Data);
            process.ErrorDataReceived += (sender, data) => Clients.Caller.SendAsync("ReceiveOutput", data.Data);
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
        }

        public record CodeRequest(string Content);
    }
}