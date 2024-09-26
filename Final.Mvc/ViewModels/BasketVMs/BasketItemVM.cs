namespace Final.Mvc.ViewModels.BasketVMs
{
    public class BasketItemVM
    {

        public int GameId { get; set; }
        public string GameTitle { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ImgUrl { get; set; }
        public decimal TotalPrice => Price * Quantity;

    }
}
