using Final.Application.Dtos.CategoryDtos;
using Final.Application.Exceptions;
using Final.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Final.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("")]
        public async Task<IActionResult> Create(CategoryCreateDto categoryCreate) => Ok(await _categoryService.Create(categoryCreate));

        [HttpGet("")]
        public async Task<IActionResult> GetAll() => Ok(await _categoryService.GetAll());

        [HttpGet("{name}")]
        public async Task<IActionResult> Get(string name)
        {
            var data = await _categoryService.GetOne(name);

            if (data is null)
            {
                throw new CustomExceptions(402, "Name", "Given category name doesnt exist.");
            }

            return Ok(data);
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                await _categoryService.Delete(name);
                return Ok($"Dlc named - '{name}' is deleted successfully");
            }
            throw new CustomExceptions(400, "Name", "Given name doesnt exist.");

        }
    }
}
