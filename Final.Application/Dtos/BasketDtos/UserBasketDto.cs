namespace Final.Application.Dtos.BasketDtos
{
    public class UserBasketDto
    {

        public string UserId { get; set; }
        public List<BasketGameDto> BasketGames { get; set; }
        public decimal TotalPrice => BasketGames?.Sum(bg => bg.Price * bg.Quantity) ?? 0;

    }
}
