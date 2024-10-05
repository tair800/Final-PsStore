using Final.Application.Dtos.PromoDtos;
using Final.Application.Exceptions;
using Final.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Final.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromoController : ControllerBase
    {
        private readonly IPromoService _promoService;

        public PromoController(IPromoService promoService)
        {
            _promoService = promoService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] PromoCreateDto promoCreateDto)
        {
            if (promoCreateDto == null)
                throw new CustomExceptions(400, "Promo data is required.");

            try
            {
                var promoId = await _promoService.Create(promoCreateDto);
                return Ok(new { success = true, promoId });
            }
            catch (CustomExceptions ex)
            {
                return BadRequest(new { success = false, code = ex.Code, message = ex.Message, errors = ex.Errors });
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var promos = await _promoService.GetAll();

                if (promos == null || !promos.Any())
                    throw new CustomExceptions(404, "Promo", "No promos found.");

                return Ok(promos);
            }
            catch (CustomExceptions ex)
            {
                return NotFound(new { success = false, code = ex.Code, message = ex.Message, errors = ex.Errors });
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id <= 0)
                    throw new CustomExceptions(400, "Id", "Invalid promo ID.");

                var promo = await _promoService.GetOne(id);
                if (promo == null)
                    throw new CustomExceptions(404, "Promo", "Promo not found.");

                return Ok(promo);
            }
            catch (CustomExceptions ex)
            {
                return BadRequest(new { success = false, code = ex.Code, message = ex.Message, errors = ex.Errors });
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                    throw new CustomExceptions(400, "Id", "Invalid promo ID.");

                await _promoService.Delete(id);
                return Ok(new { success = true, message = "Promo deleted successfully." });
            }
            catch (CustomExceptions ex)
            {
                return BadRequest(new { success = false, code = ex.Code, message = ex.Message, errors = ex.Errors });
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] PromoUpdateDto promoUpdateDto)
        {
            try
            {
                if (id <= 0)
                    throw new CustomExceptions(400, "Id", "Invalid promo ID.");

                if (promoUpdateDto == null)
                    throw new CustomExceptions(400, "Promo data is required.");

                await _promoService.Update(id, promoUpdateDto);
                return Ok(new { success = true, message = "Promo updated successfully." });
            }
            catch (CustomExceptions ex)
            {
                return BadRequest(new { success = false, code = ex.Code, message = ex.Message, errors = ex.Errors });
            }

        }
    }
}
