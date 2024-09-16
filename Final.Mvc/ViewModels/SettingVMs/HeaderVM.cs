namespace Final.Mvc.ViewModels.SettingVMs
{
    public class HeaderVM
    {
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
        public int BasketCount { get; set; }
        public decimal TotalPrice { get; set; }
        public string UserFullName { get; set; }
    }
}
