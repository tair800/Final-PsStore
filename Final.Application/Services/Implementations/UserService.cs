using AutoMapper;
using Final.Application.Dtos.UserDtos;
using Final.Application.Exceptions;
using Final.Application.Services.Interfaces;
using Final.Application.Settings;
using Final.Core.Entities;

using Final.Data.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;


namespace Final.Application.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _contextAccessor;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ITokenService tokenService, IEmailService emailService, IOptions<JwtSettings> jwtSettings, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _jwtSettings = jwtSettings.Value;
            _contextAccessor = contextAccessor;
        }

        public async Task<UserReturnDto> Register(RegisterDto registerDto, string urlScheme, string host)
        {
            var existUser = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existUser != null)
                throw new Exception("Username already exists.");

            var user = _mapper.Map<User>(registerDto);
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "member");

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string link = $"{urlScheme}://{host}/user/verifyEmail?email={user.Email}&token={token}";

            string body = await File.ReadAllTextAsync("wwwroot/templates/emailTemplate/emailConfirm.html");
            body = body.Replace("{{link}}", link).Replace("{{UserName}}", user.FullName);

            _emailService.SendEmail(new() { user.Email }, body, "Email verification", "Verify email");

            return _mapper.Map<UserReturnDto>(user);
        }

        public async Task<List<UserReturnDto>> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            if (users == null || !users.Any())
                throw new Exception("No users found.");

            var userDtos = new List<UserReturnDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userDto = _mapper.Map<UserReturnDto>(user);
                userDto.UserRoles = roles.ToList();
                userDtos.Add(userDto);
            }

            return userDtos;
        }

        public async Task<UserReturnDto> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new Exception("User not found.");

            var roles = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<UserReturnDto>(user);
            userDto.UserRoles = roles.ToList();

            return userDto;
        }

        public async Task<string> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null)
                throw new Exception("Invalid username.");

            if (user.IsBlocked)
                throw new Exception("Your account has been blocked. Please contact support.");

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result)
                throw new Exception("Invalid password.");

            var roles = await _userManager.GetRolesAsync(user);

            var token = _tokenService.GetToken(_jwtSettings.SecretKey, _jwtSettings.Audience, _jwtSettings.Issuer, user, roles);
            return token;
        }

        public async Task<bool> ChangeUserStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new Exception("User not found.");

            user.IsBlocked = !user.IsBlocked;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new Exception("Unable to change the user's status.");

            return user.IsBlocked;
        }

        public async Task<bool> EditUserRoles(EditRoleDto editRoleDto)
        {
            var user = await _userManager.FindByIdAsync(editRoleDto.UserId);
            if (user == null)
                throw new Exception("User not found.");

            var currentRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!result.Succeeded)
                throw new Exception("Failed to remove old roles.");

            result = await _userManager.AddToRolesAsync(user, editRoleDto.Roles);
            if (!result.Succeeded)
                throw new Exception("Failed to add new roles.");

            return true;
        }

        public async Task<bool> CreateRoles()
        {
            if (!await _roleManager.RoleExistsAsync("member"))
                await _roleManager.CreateAsync(new IdentityRole() { Name = "member" });

            if (!await _roleManager.RoleExistsAsync("admin"))
                await _roleManager.CreateAsync(new IdentityRole() { Name = "admin" });

            if (!await _roleManager.RoleExistsAsync("superAdmin"))
                await _roleManager.CreateAsync(new IdentityRole() { Name = "superAdmin" });

            return true;
        }

        public async Task<UserReturnDto> UpdateUser(string id, UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new Exception("User not found.");

            var passwordCheck = await _userManager.CheckPasswordAsync(user, updateUserDto.PasswordConfirmation);
            if (!passwordCheck)
                throw new Exception("Confirm password is incorrect.");

            if (await _userManager.Users.AnyAsync(u => u.UserName == updateUserDto.UserName && u.Id != user.Id))
                throw new Exception("Username is already taken.");

            _mapper.Map(updateUserDto, user);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            return _mapper.Map<UserReturnDto>(user);
        }

        public async Task<bool> VerifyEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new Exception("Invalid email.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }

        public async Task<UserReturnDto> Profile()
        {

            var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new CustomExceptions(400, "Id", "User ID cannot be null");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) throw new CustomExceptions(403, "this user doesnt exist");
            var UserMapped = _mapper.Map<UserReturnDto>(user);
            return UserMapped;

        }

    }
}
