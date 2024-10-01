namespace Final.Application.Dtos.BasketDtos
{

    public class BasketGameDto
    {
        public int GameId { get; set; }
        public string GameTitle { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string GameImgUrl { get; set; }
        public double SalePrice { get; set; }
    }
}
