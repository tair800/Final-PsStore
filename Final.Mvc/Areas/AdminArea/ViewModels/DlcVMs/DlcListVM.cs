namespace Final.Mvc.Areas.AdminArea.ViewModels.DlcVMs
{
    public class DlcListVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public string GameTitle { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedDateString => UpdatedDate.HasValue ? UpdatedDate.Value.ToString("dd-MM-yyyy") : "Not updated yet";

    }
}
