namespace Final.Application.Dtos.GameDtos
{
    public class GameUpdateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string CategoryName { get; set; }
        public int Platform { get; set; }
    }
}
