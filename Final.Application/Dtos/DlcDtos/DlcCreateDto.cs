using FluentValidation;

namespace Final.Application.Dtos.DlcDtos
{
    public class DlcCreateDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int GameId { get; set; }
    }
    public class DlcCreateDtoValidator : AbstractValidator<DlcCreateDto>
    {
        public DlcCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(3, 100).WithMessage("Name must be between 3 and 100 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.GameId)
                .GreaterThan(0).WithMessage("Must be a valid Game ID.");
        }
    }
}
