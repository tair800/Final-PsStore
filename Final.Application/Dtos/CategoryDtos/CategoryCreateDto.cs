using FluentValidation;

namespace Final.Application.Dtos.CategoryDtos
{
    public class CategoryCreateDto
    {
        public string Name { get; set; }
    }
    public class CategoryCreateDtoValidator : AbstractValidator<CategoryCreateDto>
    {
        public CategoryCreateDtoValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");

            RuleFor(c => c).Custom((c, d) =>
            {
                if (c.Name != null && !char.IsUpper(c.Name[0]))
                {
                    d.AddFailure(nameof(c.Name), "Name needs to start with uppercase");
                }
            });
        }
    }
}
