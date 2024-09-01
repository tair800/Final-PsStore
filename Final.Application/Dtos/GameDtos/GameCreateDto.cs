using Final.Core.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Final.Application.Dtos.GameDtos
{
    public class GameCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public IFormFile ImgUrl { get; set; }
        public Platform Platform { get; set; }

    }
    public class GameCreateDtoValidator : AbstractValidator<GameCreateDto>
    {
        public GameCreateDtoValidator()
        {
            RuleFor(g => g.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(g => g.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(g => g.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.")
                .ScalePrecision(2, 18).WithMessage("Price must have a maximum of 18 digits in total, with up to 2 decimal places.");

            RuleFor(g => g.CategoryId)
                .NotEmpty().WithMessage("Category is required.")
                .GreaterThan(0).WithMessage("Category ID must be greater than zero.");

            RuleFor(g => g.Platform)
                .NotEmpty().WithMessage("At least one platform must be selected.")
                .WithMessage("You must select at least one platform.");
        }
    }
}
