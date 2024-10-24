namespace Final.Mvc.ViewModels.GameVMs
{
    public class GameDetailVM
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string ImgUrl { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        //public string DlcName { get; set; }
        //public int DlcId { get; set; }
        //public string DlcImage { get; set; }
        public List<DlcDetail> DlcNames { get; set; }
        public Platform Platform { get; set; }
        public double AverageRating { get; set; }
        public int RatingCount { get; set; }
    }

    public class DlcDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
    }



    public enum Platform
    {
        PS4,
        PS5,
        PS4PS5
    }
}
