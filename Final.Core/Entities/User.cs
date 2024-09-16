using Microsoft.AspNetCore.Identity;

namespace Final.Core.Entities
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public Basket Basket { get; set; }
        public Wishlist Wishlist { get; set; }
    }
}
