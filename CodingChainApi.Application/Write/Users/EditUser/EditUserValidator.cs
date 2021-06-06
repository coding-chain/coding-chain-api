using Application.Write.Users.EditUser;
using FluentValidation;

namespace Application.Write.Users.RegisterUser
{
    public class EditUserValidator : AbstractValidator<EditUserCommand>
    {
        public EditUserValidator()
        {
            RuleFor(v => v.Email).EmailAddress();
            RuleFor(v => v.Password).MinimumLength(8);
            RuleFor(v => v.Username).MinimumLength(5);
        }
    }
}