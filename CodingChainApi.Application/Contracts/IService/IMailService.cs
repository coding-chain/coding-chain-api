using System.Threading.Tasks;

namespace Application.Contracts.IService
{
    public record Contact(string Email, string Name);

    public record Message<T>(Contact Contact, string Subject, T Content);
    public interface IMailService<TContent>
    {
        Task SendMessage(Message<TContent> message);
    }
}