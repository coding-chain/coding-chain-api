using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace CodingChainApi.Infrastructure.Hubs
{
    public record PlagiarismAnalyzeReadyEvent();

    public interface IPlagiarismClient
    {
        Task OnPlagiarismAnalyseReady(PlagiarismAnalyzeReadyEvent? serverEvent);
    }

    public class PlagiarismHub : Hub<IPlagiarismClient>
    {
        public static string Route = "/api/v1/plagiarismshub";
    }
}