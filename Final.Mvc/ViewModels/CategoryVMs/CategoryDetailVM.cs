public class CategoryDetailVM
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int GameCount { get; set; }
    public int GameId { get; set; }
    public List<string> GameNames { get; set; } = new List<string>();

}


