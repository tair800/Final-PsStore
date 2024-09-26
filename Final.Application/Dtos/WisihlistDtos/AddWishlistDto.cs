namespace Final.Application.Dtos.WishlistDtos
{
    public class AddWishlistDto
    {
        public string UserId { get; set; }
        public int GameId { get; set; }
    }

    public class RemoveWishlistDto
    {
        public string UserId { get; set; }
        public int GameId { get; set; }
    }
}
