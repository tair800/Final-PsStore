namespace Final.Application.Dtos.WishlistDtos
{

    public class UserWishlistDto
    {
        public string UserId { get; set; }
        public List<WishlistGameDto> WishlistGames { get; set; }
        //public decimal TotalPrice => WishlistGames?.Sum(wg => wg.Price) ?? 0;
    }
    public class WishlistGameDto
    {
        public int GameId { get; set; }
        public string GameTitle { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; } // Optional if applicable
        public string GameImgUrl { get; set; }
    }
}


