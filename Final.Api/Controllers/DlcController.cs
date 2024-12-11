using Final.Application.Dtos.DlcDtos;
using Final.Application.Exceptions;
using Final.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Final.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DlcController : ControllerBase
    {
        private readonly IDlcService _dlcService;

        public DlcController(IDlcService dlcService)
        {
            _dlcService = dlcService;
        }

        [HttpPost("")]
        public async Task<IActionResult> Create(DlcCreateDto dlcCreateDto) =>
            Ok(await _dlcService.Create(dlcCreateDto));

        [HttpGet("")]
        public async Task<IActionResult> GetAll() =>
            Ok(await _dlcService.GetAll());

        [HttpGet("ForAdmin")]
        [Authorize(Roles = "admin,superAdmin")]
        public async Task<IActionResult> GetAllAdmin() =>
        Ok(await _dlcService.GetAll());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var data = await _dlcService.GetOne(id);
            if (data == null)
            {
                throw new CustomExceptions(400, "Name", "Given dlc doesnt exist.");
            }
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id > 0)
            {
                await _dlcService.Delete(id);
                return Ok($"Dlc - '{id}' is deleted successfully");
            }
            throw new CustomExceptions(400, "Id", "Given id doesnt exist.");

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] DlcUpdateDto dlcUpdateDto)
        {
            if (id <= 0)
            {
                throw new CustomExceptions(400, "Id", "Invalid DLC id.");
            }

            // Call the service to update the DLC
            await _dlcService.Update(id, dlcUpdateDto);

            return Ok("Dlc updated successfully.");
        }
        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var count = await _dlcService.GetCount();
            return Ok(new { Count = count });
        }



    }
}
