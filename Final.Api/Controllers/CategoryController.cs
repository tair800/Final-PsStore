﻿using Final.Application.Dtos.CategoryDtos;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var data = await _categoryService.GetOne(id);

            if (data is null)
            {
                throw new CustomExceptions(402, "Name", "Given category name doesnt exist.");
            }

            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id > 0)
            {
                await _categoryService.Delete(id);
                return Ok($"Dlc  - '{id}' is deleted successfully");
            }
            throw new CustomExceptions(400, "Name", "Given name doesnt exist.");

        }
    }
}
