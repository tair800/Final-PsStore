using Final.Application.Dtos.UserDtos;

namespace Final.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserReturnDto> GetUserByEmail(string email);
        Task<UserReturnDto> CreateUser(RegisterDto registerDto);
        Task<bool> DeleteUser(string email);
        Task<UserReturnDto> UpdateUser(string email, UpdateUserDto updateUserDto);
        Task<List<UserReturnDto>> GetAllUsers();
    }
}
