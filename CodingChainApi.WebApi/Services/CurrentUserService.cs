using System;
using System.Security.Claims;
using Application.Contracts.IService;
using Domain.Users;
using Microsoft.AspNetCore.Http;

namespace CodingChainApi.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private UserId? _userId;

        public UserId UserId
        {
            get
            {
                if (_userId is not null) return _userId;
                var id = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (id is null)
                    throw new ApplicationException("User not authenticated");
                return new UserId(Guid.Parse(id));
            }
            set => _userId = value;
        }
    }
}