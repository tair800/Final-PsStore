using Final.Core.Entities;

namespace Final.Application.Dtos.BasketDtos
{
    public class BasketDto
    {
        public string UserId { get; set; }
        public List<BasketGameDto> Games { get; set; }
    }

    public class BasketGameDto
    {
        public int id { get; set; }
        public Game Game { get; set; }
        public string GameTitle { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
