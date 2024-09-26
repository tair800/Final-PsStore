using Final.Application.Dtos.WishlistDtos;

namespace Final.Application.Dtos.WishlistDtos
{
    public class WishlistDto
    {
        public string UserId { get; set; }
        public List<WishlistGameDto> WishlistGames { get; set; }
    }

    public class WishlistGameDto
    {
        public int GameId { get; set; }
        public string GameTitle { get; set; }
        public decimal Price { get; set; }
    }
}

public class UserWishlistDto
{
    public string UserId { get; set; }
    public List<WishlistGameDto> WishlistGames { get; set; }
    public decimal TotalPrice => WishlistGames?.Sum(wg => wg.Price) ?? 0;
}
