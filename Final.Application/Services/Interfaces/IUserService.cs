using Final.Application.Dtos.UserDtos;
using Final.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Final.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterDto registerDto, string requestScheme, string requestHost);
        Task<List<UserReturnDto>> GetAllUsers();
        Task<UserReturnDto> GetUserById(string id);
        Task<string> Login(LoginDto loginDto);
        Task<bool> ChangeUserStatus(string id);
        Task<bool> EditUserRoles(EditRoleDto editRoleDto);
        Task<bool> CreateRoles();
        Task<UserReturnDto> UpdateUser(string id, UpdateUserDto updateUserDto);
        Task VerifyEmailAsync(string email, string token);
        Task<ForgotPasswordDto> ForgotPassword(ForgotPasswordDto forgotPasswordDto);
        Task<bool> ResetPassword(ResetPasswordDto resetPasswordDto);
        Task<List<string>> GetAllRoles();
        Task<bool> ConfirmEmail(string email, string token);
        Task<List<UserReturnDto>> GetVerifiedUsersAsync();
        Task<bool> AdminPasswordChange(string userId, string newPassword);
        Task<bool> SaveCard(string userId, SaveCardDto cardDto);
        Task<List<UserCard>> GetUserCards(string userId);
    }

}

