using Final.Core.Entities.Common;

namespace Final.Core.Entities
{
    public class UserCard : BaseEntity
    {
        public string UserId { get; set; }  // Foreign key to the User
        public string CardNumber { get; set; }  // Masked card number
        public string Last4Digits { get; set; }  // Last 4 digits of the card number
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string CVC { get; set; }  // Storing this might not be recommended for security reasons

        // Navigation property to User (optional)
        public User User { get; set; }
    }
}
