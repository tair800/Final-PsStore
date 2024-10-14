namespace Final.Application.Dtos.OrderDtos
{
    public class OrderItemDto
    {
        public int GameId { get; set; }
        public string GameTitle { get; set; }  // Game title for easy reference
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

}
