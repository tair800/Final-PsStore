using Microsoft.AspNetCore.Identity;

namespace Final.Core.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Basket Basket { get; set; }
        public Wishlist Wishlist { get; set; }
    }
}
