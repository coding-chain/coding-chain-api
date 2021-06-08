using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using Application.Read.Rights;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace CodingChainApi.Infrastructure.Repositories.ReadRepositories
{
    public class ReadRightRepository : IReadRightRepository
    {
        private readonly CodingChainContext _context;

        public ReadRightRepository(CodingChainContext context)
        {
            _context = context;
        }

        public async Task<IPagedList<RightNavigation>> GetAllRightNavigationPaginated(
            PaginationQueryBase paginationQuery)
        {
            return await _context.Rights.Select(r => ToRightNavigation(r)).FromPaginationQueryAsync(paginationQuery);
        }

        public async Task<RightNavigation?> GetOneRightNavigationById(Guid id)
        {
            var right = await _context.Rights.FirstOrDefaultAsync(r => r.Id == id);
            return right is null ? null : ToRightNavigation(right);
        }

        private static RightNavigation ToRightNavigation(Right right)
        {
            return new(right.Id, right.Name);
        }
    }
}