using Microsoft.AspNetCore.Http;

namespace Final.Application.Dtos.DlcDtos
{
    public class DlcUpdateDto
    {
        public string? Name { get; set; }
        public double? Price { get; set; }
        public int? GameId { get; set; }
        public string? Image { get; set; }  // This stores the path of the existing image
        public IFormFile? File { get; set; }

    }


}
