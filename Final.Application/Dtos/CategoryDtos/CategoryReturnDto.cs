namespace Final.Application.Dtos.CategoryDtos
{
    public class CategoryReturnDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> GameNames { get; set; } = new List<string>();
        public int GameCount => GameNames.Count;
    }
}
