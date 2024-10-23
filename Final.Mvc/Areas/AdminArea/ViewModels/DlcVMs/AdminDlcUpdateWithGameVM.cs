using Final.Mvc.Areas.AdminArea.ViewModels.GameVMs;

namespace Final.Mvc.Areas.AdminArea.ViewModels.DlcVMs
{
    public class AdminDlcUpdateWithGameVM
    {
        public AdminDlcUpdateVM Dlc { get; set; }  // The DLC data to be updated
        public List<GameListVM> Games { get; set; }
    }
}
