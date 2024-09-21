public class RoleVM
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string SelectedRole { get; set; }
    public List<string> AvailableRoles { get; set; } = new List<string>();
    public List<string> UserRoles { get; set; } = new List<string>();  // Roles assigned to the user
}
