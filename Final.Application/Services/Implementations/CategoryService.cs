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
        private readonly IActivityService _activityService;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IActivityService activityService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _activityService = activityService;
        }

        public async Task<int> Create(CategoryCreateDto categoryCreateDto)
        {
            if (await _unitOfWork.categoryRepository.isExists(c => c.Name.ToLower() == categoryCreateDto.Name.ToLower()))
                throw new CustomExceptions(400, "Name", "Duplicate is not permitted");

            var category = _mapper.Map<Category>(categoryCreateDto);
            await _unitOfWork.categoryRepository.Create(category);
            _unitOfWork.Commit();

            // Log the create activity
            await _activityService.LogActivity(new Activ
            {
                ActionType = "Create",
                EntityName = "Category",
                Details = $"Category '{category.Name}' was created.",
                PerformedBy = "System"
            });

            return category.Id;
        }

        public async Task Delete(int id)
        {
            var category = await _unitOfWork.categoryRepository.GetEntity(c => c.Id == id);
            if (category == null)
                throw new CustomExceptions(404, "Category", "Category not found.");

            await _unitOfWork.categoryRepository.Delete(category);
            _unitOfWork.Commit();

            // Log the delete activity
            await _activityService.LogActivity(new Activ
            {
                ActionType = "Delete",
                EntityName = "Category",
                Details = $"Category '{category.Name}' with ID {id} was deleted.",
                PerformedBy = "System"
            });
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

            var oldName = category.Name; // Capture old state for logging

            category.UpdatedDate = DateTime.UtcNow;
            _mapper.Map(updateDto, category);

            try
            {
                await _unitOfWork.categoryRepository.Update(category);
                _unitOfWork.Commit(); // Ensure this call is working correctly
            }
            catch (Exception ex)
            {
                // Log or handle the error here
                throw new CustomExceptions(500, "Update", $"An error occurred while saving changes: {ex.Message}");
            }

            // Log the update activity
            try
            {
                await _activityService.LogActivity(new Activ
                {
                    ActionType = "Update",
                    EntityName = "Category",
                    Details = $"Category '{oldName}' was updated to '{category.Name}'.",
                    PerformedBy = "System"
                });
            }
            catch (Exception ex)
            {
                // Optionally log this failure somewhere; it should not affect the main update
                Console.WriteLine($"Failed to log activity: {ex.Message}");
            }
        }

    }
}
