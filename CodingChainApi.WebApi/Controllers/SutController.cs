using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CodingChainApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SutController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post()
        {
            var cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine("dotnet test ../csharp_template");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            Console.WriteLine(cmd.StandardOutput.ReadToEnd());
            return Ok();
        }
    }
}