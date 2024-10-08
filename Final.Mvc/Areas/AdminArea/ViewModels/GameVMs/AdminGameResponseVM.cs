namespace Final.Mvc.Areas.AdminArea.ViewModels.GameVMs
{
    public class AdminGameResponseVM
    {
        public int Id { get; set; }           // The ID of the created game
        public string Title { get; set; }      // The title of the game
        public string Description { get; set; } // A brief description of the game
        public decimal Price { get; set; }     // The price of the game
        public decimal? SalePrice { get; set; } // Optional sale price of the game
        public string ImgUrl { get; set; }     // The URL of the game's image
        public int CategoryId { get; set; }    // The ID of the category to which the game belongs
        public string CategoryName { get; set; } // The name of the category
        public Platform Platform { get; set; } // The platform (PS4, PS5, etc.)
        public DateTime CreatedDate { get; set; } // The date the game was created
        public DateTime? UpdatedDate { get; set; } // The date the game was last updated (optional)
    }

}
