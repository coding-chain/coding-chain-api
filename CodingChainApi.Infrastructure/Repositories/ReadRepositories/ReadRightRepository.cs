using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Pagination;
using Application.Read.Contracts;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Right = CodingChainApi.Infrastructure.Models.Right;
using RightNavigation = Application.Read.Rights.RightNavigation;

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

        public async Task<RightNavigation?> GetOneRightNavigationByName(RightEnum name)
        {
            var right = await _context.Rights.FirstOrDefaultAsync(r => r.Name == name);
            return right is null ? null : ToRightNavigation(right);
        }

        private static RightNavigation ToRightNavigation(Right right)
        {
            return new(right.Id, right.Name);
        }
    }
}