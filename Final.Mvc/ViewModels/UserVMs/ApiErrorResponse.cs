namespace Final.Mvc.ViewModels.UserVMs
{
    public class ApiErrorResponse
    {
        public string Message { get; set; }
        public Dictionary<string, string> Errors { get; set; }
    }
}
