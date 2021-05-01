using FluentValidation;

namespace Application.Write.Users.LoginUser
{
    public class LoginUserValidator : AbstractValidator<LoginUserQuery>
    {
        public LoginUserValidator()
        {
            RuleFor(v => v.Email).EmailAddress().NotEmpty();
            RuleFor(v => v.Password).NotEmpty().MinimumLength(8);
        }
    }
}