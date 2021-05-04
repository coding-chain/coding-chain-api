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

        public UserId? UserId
        {
            get
            {
                var id = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                return id != null ? new UserId(Guid.Parse(id)) : null;
            } 
        }

        private UserId? _connectedUserId;

        public UserId ConnectedUserId
        {
            get => _connectedUserId ?? throw new ApplicationException("User not authenticated"); set => _connectedUserId = value;
        }
    }
}