namespace Final.Application.Exceptions
{
    public class CustomExceptions : Exception
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string> Errors { get; set; } = new();

        public CustomExceptions(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public CustomExceptions(string errorKey, string errorMessage)
        {
            Errors.Add(errorKey, errorMessage);
        }

        public CustomExceptions(int code, string errorKey, string errorMessage, string message = null)
        {
            Code = code;
            Message = message;
            Errors.Add(errorKey, errorMessage);
        }
    }
}
