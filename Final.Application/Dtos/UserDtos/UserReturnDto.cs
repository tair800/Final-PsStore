namespace Final.Application.Dtos.UserDtos
{
    public class UserReturnDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsBlocked { get; set; }
        public List<string> UserRoles { get; set; } = new List<string>();
        public string Token { get; set; }
        public bool IsVerified { get; set; }

    }
}
