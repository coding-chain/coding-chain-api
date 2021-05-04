using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Write.Contracts;
using CodingChainApi.Infrastructure.Common.Extensions;
using CodingChainApi.Infrastructure.Contexts;
using CodingChainApi.Infrastructure.Models;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Right = Domain.Users.Right;

namespace CodingChainApi.Infrastructure.Repositories.AggregateRepositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CodingChainContext _context;

        public UserRepository(CodingChainContext context)
        {
            _context = context;
        }

        private async Task<Models.User?> FindAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => !u.IsDeleted && u.Id == id);
        }

        public async Task<UserId> SetAsync(UserAggregate aggregate)
        {
            var user = await ToModel(aggregate);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return new UserId(user.Id);
        }


        private async Task<Models.User> ToModel(UserAggregate aggregate)
        {
            var rightsNames = aggregate.Rights.Select(r => r.Name);
            var user = await FindAsync(aggregate.Id.Value) ?? new Models.User();
            user.Email = aggregate.Email;
            user.Password = aggregate.Password;
            user.Rights = await _context.Rights.Where(r => rightsNames.Contains(r.Name)).ToListAsync();
            user.Username = aggregate.Username;
            user.Id = aggregate.Id.Value;
            return user;
        }

        private static UserAggregate ToEntity(Models.User model)
        {
            return new(
                new UserId(model.Id),
                model.Password,
                model.Rights.Select(r => new Right(r.Name)).ToList(),
                model.Email,
                model.Username);
        }

        public async Task<UserAggregate?> FindByIdAsync(UserId id)
        {
            return ToEntity(await _context.Users
                .Include(u => u.Rights)
                .FirstOrDefaultAsync(u => u.Id == id.Value && !u.IsDeleted ));
        }

        public async Task RemoveAsync(UserId id)
        {
            var user = await FindAsync(id.Value);
            user.IsDeleted = true;
        }

        public Task<UserId> NextIdAsync()
        {
            return new UserId(new Guid()).ToTask();
        }

        public async Task<IList<UserAggregate>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Rights)
                .Select(u => ToEntity(u))
                .ToListAsync();
        }
    }
}