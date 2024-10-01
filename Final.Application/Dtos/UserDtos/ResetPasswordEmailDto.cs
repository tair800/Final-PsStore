namespace Final.Application.Dtos.UserDtos
{
    public class ResetPasswordEmailDto
    {
        public string Email { get; set; }
        public string? Token { get; set; }
    }
}
