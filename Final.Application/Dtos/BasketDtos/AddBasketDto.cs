namespace Final.Application.Dtos.BasketDtos
{
    public class AddBasketDto
    {
        public string UserId { get; set; }
        public int GameId { get; set; }
        public int Quantity { get; set; }
    }
}
