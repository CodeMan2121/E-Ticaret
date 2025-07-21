using FluentValidation;

namespace FirstProject.Models
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator() {
            RuleFor(u=>u.FirstName).NotEmpty().NotNull();
            RuleFor(u=>u.LastName).NotEmpty().NotNull();
            RuleFor(u=>u.Email).NotEmpty().NotNull().EmailAddress();
            RuleFor(u=>u.Password).NotEmpty().NotNull().MaximumLength(25).MinimumLength(6);
        }
    }
}
