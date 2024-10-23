namespace Final.Mvc.ViewModels.BasketVMs
{
    public class BasketVM
    {

        public int Id { get; set; }
        public string UserId { get; set; } // The ID of the user who owns the basket
        public List<BasketGameViewModel> BasketGames { get; set; } // List of games in the basket
        public decimal TotalPrice { get; set; } // The total price of all items in the basket

    }
    public class BasketGameViewModel
    {
        public int GameId { get; set; } // The ID of the game
        public string GameTitle { get; set; } // The title of the game
        public int Quantity { get; set; } // The number of copies in the basket
        public decimal Price { get; set; } // The price of the game
    }

}
