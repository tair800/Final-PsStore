using Microsoft.AspNetCore.Identity;

namespace Final.Core.Entities
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public bool IsBlocked { get; set; }

        public Basket Basket { get; set; }
        public Wishlist Wishlist { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Rating> Ratings { get; set; }
        public ICollection<UserCard> UserCards { get; set; }  // Collection of saved cards for the user

    }
}
