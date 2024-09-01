using FluentValidation;

namespace Final.Application.Dtos.UserDtos
{
    public class LoginDto
    {
        public string UserName { get; set; }
        //public string Email { get; set; }
        public string Password { get; set; }
    }
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(c => c.UserName)
                .NotNull()
                .MaximumLength(30);

            //RuleFor(c => c.Email)
            //   .NotNull()
            //   .EmailAddress()
            //   .MaximumLength(30);

            RuleFor(c => c.Password)
                .NotNull()
                .MaximumLength(12)
                .MinimumLength(6);

        }
    }
}
