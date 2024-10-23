namespace Final.Application.Dtos.OrderDtos
{
    public class OrderReturnDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }  //
    }
}
