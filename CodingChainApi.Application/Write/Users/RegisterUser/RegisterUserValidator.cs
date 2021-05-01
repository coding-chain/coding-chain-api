using FluentValidation;

namespace Application.Write.Users.RegisterUser
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserValidator()
        {
            RuleFor(v => v.Email).EmailAddress().NotEmpty();
            RuleFor(v => v.Password).NotEmpty().MinimumLength(8);
            RuleFor(v => v.Username).NotEmpty().MinimumLength(5);
        }
    }
}