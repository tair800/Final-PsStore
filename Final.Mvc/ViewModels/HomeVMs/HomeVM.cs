using Final.Mvc.Areas.AdminArea.ViewModels.GameVMs;
using Final.Mvc.ViewModels.GameVMs;
using Final.Mvc.ViewModels.PromoVMs;

namespace Final.Mvc.ViewModels.HomeVMs
{
    public class HomeVM
    {
        public List<GameListItemVM> Games { get; set; }
        public List<CategoryVM> Categories { get; set; }
        public List<PromoVM> Promos { get; set; }
        public List<GameListItemVM> Deals { get; set; }
        public List<GameListItemVM> NewGames { get; set; }
    }
}
