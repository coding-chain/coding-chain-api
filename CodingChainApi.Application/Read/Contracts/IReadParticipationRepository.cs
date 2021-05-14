using System;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Participations;

namespace Application.Read.Contracts
{
    public interface  IReadParticipationRepository
    {
        public Task<IPagedList<ParticipationNavigation>> GetAllParticipationNavigationPaginated(PaginationQueryBase paginationQuery);
        public Task<ParticipationNavigation?> GetOneParticipationNavigationById(Guid id);
    }
}