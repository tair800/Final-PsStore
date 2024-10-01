namespace Final.Mvc.ViewModels.UserVMs
{
    public class ResetPasswordEmailVM
    {
        public EmailTokenVM Message { get; set; }

    }
    public class EmailTokenVM
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
