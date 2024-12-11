namespace Final.Mvc.ViewModels.WishlistVMs
{
    public class UserWishlistVM
    {
        public string UserId { get; set; }
        public IEnumerable<WishlistGameVM> WishlistGames { get; set; } // Ensure this is IEnumerable
    }



}
