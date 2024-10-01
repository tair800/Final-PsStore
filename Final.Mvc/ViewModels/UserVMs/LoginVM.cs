using FluentValidation;

namespace Final.Mvc.ViewModels.UserVMs
{
    public class LoginVM
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class LoginVMValidator : AbstractValidator<LoginVM>
    {
        public LoginVMValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
