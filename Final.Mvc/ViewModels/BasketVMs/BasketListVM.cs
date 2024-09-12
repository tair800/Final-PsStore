public class BasketListVM
{
    public List<BasketGame> BasketGames { get; set; }
}

public class BasketGame
{
    public Game Game { get; set; }
    public int Quantity { get; set; }
}

public class Game
{
    public string Title { get; set; }
    public string Price { get; set; }
    public string? SalePrice { get; set; }

}
