namespace Final.Application.Dtos.UserDtos
{
    public class UpdateUserDto
    {

        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string PasswordConfirmation { get; set; }
    }
}
