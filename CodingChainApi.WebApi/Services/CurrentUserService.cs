using System;
using System.Security.Claims;
using Application.Contracts.IService;
using Domain.Users;
using Microsoft.AspNetCore.Http;

namespace NeosCodingApi.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UserId UserId
        {
            get
            {
                var id = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (id is null)
                    throw new ApplicationException("User not authenticated");
                return new UserId(Guid.Parse(id));
            }
        }
    }
}