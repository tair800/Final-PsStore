using Final.Core.Entities;

namespace Final.Application.ViewModels
{
    public class AdminGameReturnVM
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
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public List<AdminDlcVm> DlcNames { get; set; } = new List<AdminDlcVm>(); // Use List of DLC ViewModel for displaying DLCs

        // Additional properties for admin use
        //public bool IsOnSale => SalePrice.HasValue && SalePrice.Value < Price;
        //public int TotalDlcCount => DlcList.Count;
    }

    public class AdminDlcVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
