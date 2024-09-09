namespace Final.Application.Dtos.BasketDtos
{
    public class BasketDto
    {
        public string UserId { get; set; }
        public List<BasketGameDto> Games { get; set; }
    }

    public class BasketGameDto
    {
        public string GameName { get; set; }
        public decimal GamePrice { get; set; }
        public int Quantity { get; set; }
    }
}
