namespace Final.Mvc.Areas.AdminArea.ViewModels.UserVMs
{
    public class UserReturnVM
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsBlocked { get; set; }
        public List<string> UserRoles { get; set; }
        public bool IsVerified { get; set; }
    }
}
