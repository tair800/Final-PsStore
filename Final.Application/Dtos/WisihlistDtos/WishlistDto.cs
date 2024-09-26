namespace Final.Application.Dtos.WisihlistDtos
{
    public class WishlistDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();
    }

    public class WishlistItem
    {
        public int Id { get; set; }
        public string GameTitle { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public string ImgUrl { get; set; }
    }
}
