using FluentValidation;

namespace Application.Write.Users.EditUser
{
    public class EditUserValidator : AbstractValidator<EditUserCommand>
    {
        public EditUserValidator()
        {
            RuleFor(cmd => cmd.Email).EmailAddress().When(cmd => cmd.Email is not null);
            RuleFor(cmd => cmd.Password).MinimumLength(8).When(cmd => cmd.Password is not null);
            RuleFor(cmd => cmd.Username).MinimumLength(5).When(cmd => cmd.Username is not null);
        }
    }
}