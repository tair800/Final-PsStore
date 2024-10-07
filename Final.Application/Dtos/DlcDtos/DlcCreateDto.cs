using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Final.Application.Dtos.DlcDtos
{
    public class DlcCreateDto
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int GameId { get; set; }
        public IFormFile Image { get; set; }
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

            RuleFor(x => x.Image)
                .NotNull().WithMessage("Image is required.")
                .Must(file => file.Length > 0).WithMessage("Uploaded file is empty.")
                .Must(file => file.ContentType.StartsWith("image/")).WithMessage("Only image files are allowed.");
        }
    }
}
