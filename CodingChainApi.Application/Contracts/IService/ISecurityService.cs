namespace Application.Contracts.IService
{
    public interface ISecurityService
    {
        public string HashPassword(string password);
        public bool ValidatePassword(string clearPassword, string hashedPassword);
    }
}