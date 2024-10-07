using Microsoft.AspNetCore.Http;

namespace Final.Application.Dtos.DlcDtos
{
    public class DlcUpdateDto
    {
        public string? Name { get; set; }
        public double? Price { get; set; }
        //public int? GameId { get; set; }
        public IFormFile? File { get; set; } // Added IFormFile to handle image upload

    }

}
