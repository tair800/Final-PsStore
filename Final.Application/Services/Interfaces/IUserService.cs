using Final.Application.Dtos.UserDtos;

namespace Final.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserReturnDto> Register(RegisterDto registerDto, string urlScheme, string host);
        Task<List<UserReturnDto>> GetAllUsers();
        Task<UserReturnDto> GetUserById(string id);
        Task<string> Login(LoginDto loginDto);
        Task<bool> ChangeUserStatus(string id);
        Task<bool> EditUserRoles(EditRoleDto editRoleDto);
        Task<bool> CreateRoles();
        Task<UserReturnDto> UpdateUser(string id, UpdateUserDto updateUserDto);
        Task<bool> VerifyEmail(string email, string token);
        Task<ForgotPasswordDto> ForgotPassword(ForgotPasswordDto forgotPasswordDto);
        Task<bool> ResetPassword(ResetPasswordDto resetPasswordDto);
        Task<List<string>> GetAllRoles();
        Task<bool> ConfirmEmail(string email, string token);
        Task<List<UserReturnDto>> GetVerifiedUsersAsync();

    }

}

