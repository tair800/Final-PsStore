using Final.Core.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Final.Application.Dtos.GameDtos
{
    public class GameUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? SalePrice { get; set; }
        //public string? ImgUrl { get; set; }
        public IFormFile? File { get; set; }
        public int? CategoryId { get; set; }
        public Platform? Platform { get; set; }
    }
    public class GameUpdateDtoValidator : AbstractValidator<GameUpdateDto>
    {
        public GameUpdateDtoValidator()
        {


            RuleFor(g => g.SalePrice)
          .LessThanOrEqualTo(g => g.Price).When(g => g.SalePrice.HasValue)
          .WithMessage("Sale price cannot be greater than the regular price.");


        }
    }



}
