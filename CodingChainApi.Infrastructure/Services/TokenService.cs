using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.IService;
using CodingChainApi.Infrastructure.Common.Exceptions;
using CodingChainApi.Infrastructure.Settings;
using Domain.Users;
using Microsoft.IdentityModel.Tokens;

namespace CodingChainApi.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IJwtSettings _settings;
        private const int DefaultMinutesDuration = 120; 
        public TokenService(IJwtSettings settings)
        {
            _settings = settings;
        }

        public Task<string> GenerateUserTokenAsync(UserAggregate user)
        {
            if (_settings.Key is null)
                throw new InfrastructureException("JWt Key is null please check your JwtSettings");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new JwtSecurityToken(_settings.Issuer,
                _settings.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(_settings.MinutesDuration ?? DefaultMinutesDuration),
                signingCredentials: credentials);
            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}