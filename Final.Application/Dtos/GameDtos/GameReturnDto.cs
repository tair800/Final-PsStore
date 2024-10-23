using Final.Core.Entities;

namespace Final.Application.Dtos.GameDtos
{
    public class GameReturnDto
    {
        public int Id { get; set; }
        public string ImgUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public Platform Platform { get; set; }
        //public int DlcId { get; set; }
        public List<DlcReturnDto> DlcNames { get; set; } = new List<DlcReturnDto>();
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsInWishlist { get; set; }
        public double AverageRating { get; set; }

        public double RatingCount { get; set; }
    }

}
