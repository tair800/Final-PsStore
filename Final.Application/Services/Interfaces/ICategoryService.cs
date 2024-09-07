using Final.Application.Dtos.CategoryDtos;

namespace Final.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<int> Create(CategoryCreateDto categoryCreateDto);
        Task<List<CategoryReturnDto>> GetAll();
        Task Delete(int id);
        Task<CategoryReturnDto> GetOne(int id);
    }
}
