namespace Final.Mvc.Areas.AdminArea.ViewModels.GameVMs
{
    public class AdminGameReturnVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }

        public int DlcId { get; set; }
        public List<string> DlcNames { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Platform { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
