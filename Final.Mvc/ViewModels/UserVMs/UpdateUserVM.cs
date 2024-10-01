using FluentValidation;

public class UpdateUserVM
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? FullName { get; set; }
    public string Email { get; set; }
    public List<string> UserRoles { get; set; }
    public string? NewPassword { get; set; }
    public string? VerifyNewPassword { get; set; }
    public string? CurrentPassword { get; set; }
}
public class UpdateUserVMValidator : AbstractValidator<UpdateUserVM>
{
    public UpdateUserVMValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.UserRoles)
            .NotNull().WithMessage("User roles cannot be null.")
            .Must(roles => roles.Count > 0).WithMessage("At least one role must be specified.");

        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required for updates.")
            .When(x => !string.IsNullOrEmpty(x.NewPassword) || !string.IsNullOrEmpty(x.VerifyNewPassword))
            .WithMessage("Current password is required to set a new password.");

        RuleFor(x => x.NewPassword)
            .Equal(x => x.VerifyNewPassword).WithMessage("New password and verify password must match.")
            .When(x => !string.IsNullOrEmpty(x.NewPassword));

        RuleFor(x => x.VerifyNewPassword)
            .Equal(x => x.NewPassword).WithMessage("Verify password must match the new password.")
            .When(x => !string.IsNullOrEmpty(x.VerifyNewPassword));
    }
}