using Final.Application.Dtos.DlcDtos;
using Final.Application.Exceptions;
using Final.Application.Services.Interfaces;
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

        [HttpGet("{name}")]
        public async Task<IActionResult> Get(string name)
        {
            var data = await _dlcService.GetOne(name);
            if (data == null)
            {
                throw new CustomExceptions(400, "Name", "Given dlc doesnt exist.");
            }
            return Ok(data);
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                await _dlcService.Delete(name);
                return Ok($"Dlc named - '{name}' is deleted successfully");
            }
            throw new CustomExceptions(400, "Name", "Given name doesnt exist.");

        }


    }
}
