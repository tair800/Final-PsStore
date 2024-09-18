namespace Final.Mvc.ViewModels.GameVMs
{
    public class GameReturnVM
    {
        public int Id { get; set; }
        public int Page { get; set; }
        public int TotalCount { get; set; }
        public List<GameListItemVM> Items { get; set; }
    }

    public class GameListItemVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string ImgUrl { get; set; } // Updated to match the DTO
        public Platform Platform { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; } // Added to map with the DTO
        public int DlcId { get; set; } // Added to map with the DTO
        public List<string> DlcNames { get; set; } // Added to map with the DTO
    }

    public class CategoryInGame
    {
        public string Name { get; set; }
        public int GamesCount { get; set; }
    }
}
