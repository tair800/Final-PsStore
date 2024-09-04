using Final.Application.Dtos.GameDtos;

namespace Final.Application.Dtos.CategoryDtos
{
    public class CategoryReturnDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<GameReturnDto> Games { get; set; }
    }
}
