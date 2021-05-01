using System.Threading.Tasks;
using Domain.Contracts;

namespace Application.Common.Events
{
    public interface IDomainEventDispatcher
    {
        Task Dispatch(IDomainEvent domainEvent);
    }
}