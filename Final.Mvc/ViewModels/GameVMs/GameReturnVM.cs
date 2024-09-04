﻿using Final.Core.Entities;

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
        public Platform Platform { get; set; }

        public decimal? SalePrice { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public CategoryInGame Category { get; set; }
    }

    public class CategoryInGame
    {
        public string Name { get; set; }
        public int GamesCount { get; set; }
    }
}