using Application.Contracts.IService;
using CodingChainApi.Infrastructure.Settings;
using static BCrypt.Net.BCrypt;

namespace CodingChainApi.Infrastructure.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly IBcryptSettings _settings;

        public SecurityService(IBcryptSettings settings)
        {
            _settings = settings;
        }

        private string Salt => GenerateSalt(_settings.WorkFactor);

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, Salt);
        }

        public bool ValidatePassword(string clearPassword, string hashedPassword)
        {
            return Verify(clearPassword, hashedPassword);
        }
    }
}