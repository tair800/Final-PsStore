using Final.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace Final.Application.Dtos.GameDtos
{
    public class GameUpdateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        //public string ImgUrl { get; set; }
        public IFormFile File { get; set; }
        public int CategoryId { get; set; }
        public Platform Platform { get; set; }
    }
}
