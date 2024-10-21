namespace Final.Mvc.ViewModels.GameVMs
{
    public class PaginationVM<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public List<T> Data { get; set; }
    }
}
