namespace Final.Application.Dtos.OrderDtos
{
    public class OrderDto
    {
        public int Id { get; set; }  // Order ID
        public string UserId { get; set; }  // User ID
        public decimal TotalPrice { get; set; }  // Total Price of the Order
        public DateTime OrderDate { get; set; }  // Date of the Order
        public List<OrderItemDto> OrderItems { get; set; }  // Lis
    }

}
