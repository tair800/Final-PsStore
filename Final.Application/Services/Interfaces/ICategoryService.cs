using Final.Application.Dtos.CategoryDtos;
using Final.Core.Entities;

namespace Final.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<int> Create(CategoryCreateDto categoryCreateDto);
        Task<List<Category>> GetAll();

        Task<Category> GetOne(string name);
    }
}
