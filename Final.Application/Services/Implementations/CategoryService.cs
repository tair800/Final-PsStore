using AutoMapper;
using Final.Application.Dtos.CategoryDtos;
using Final.Application.Exceptions;
using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Final.Data.Implementations;

namespace Final.Application.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> Create(CategoryCreateDto categoryCreateDto)
        {
            if (await _unitOfWork.categoryRepository.isExists(c => c.Name.ToLower() == categoryCreateDto.Name.ToLower()))
                throw new CustomExceptions(400, "Name", "Dublicate is not permitted");

            var category = _mapper.Map<Category>(categoryCreateDto);
            await _unitOfWork.categoryRepository.Create(category);
            _unitOfWork.Commit();

            return category.Id;
        }

        public async Task<List<Category>> GetAll()
        {
            return await _unitOfWork.categoryRepository.GetAll();
        }

        public async Task<Category> GetOne(string name)
        {
            return await _unitOfWork.categoryRepository.GetEntity(g => g.Name == name, "Games");
        }
    }
}
