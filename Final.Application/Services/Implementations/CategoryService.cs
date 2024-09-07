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

        public async Task Delete(int id)
        {
            var category = await _unitOfWork.categoryRepository.GetEntity(c => c.Id == id);
            if (category == null)
                throw new CustomExceptions(404, "Category", "Category not found.");

            await _unitOfWork.categoryRepository.Delete(category);
            _unitOfWork.Commit();

        }

        public async Task<List<CategoryReturnDto>> GetAll()
        {
            var categories = await _unitOfWork.categoryRepository.GetAll(null, "Games");
            return _mapper.Map<List<CategoryReturnDto>>(categories);

        }


        public async Task<CategoryReturnDto> GetOne(int id)
        {
            var category = await _unitOfWork.categoryRepository.GetEntity(g => g.Id == id, "Games");

            if (category == null)
            {
                throw new CustomExceptions(404, "Category", "Category not found.");
            }

            var categoryDto = _mapper.Map<CategoryReturnDto>(category);

            return categoryDto;
        }

        public async Task Update(int id, CategoryUpdateDto updateDto)
        {
            var category = await _unitOfWork.categoryRepository.GetEntity(c => c.Id == id);

            if (category == null)
                throw new CustomExceptions(404, "Category", "Category not found.");


            category.UpdatedDate = DateTime.UtcNow;


            _mapper.Map(updateDto, category);

            await _unitOfWork.categoryRepository.Update(category);
            _unitOfWork.Commit();
        }
    }
}
