using Microsoft.AspNetCore.Http;

namespace Final.Application.Dtos.PromoDtos
{
    public class PromoCreateDto
    {
        public string Name { get; set; }
        public IFormFile File { get; set; }
    }
}
