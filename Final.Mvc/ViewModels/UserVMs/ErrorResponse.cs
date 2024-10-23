namespace Final.Mvc.ViewModels.UserVMs
{
    public class ErrorResponse
    {
        public string Message { get; set; }  // For the general error message
        public Dictionary<string, List<string>> Errors { get; set; }  // For field-specific validation errors
    }

}
