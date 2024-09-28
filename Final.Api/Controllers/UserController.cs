using AutoMapper;
using Final.Application.Dtos.UserDtos;
using Final.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Final.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var user = await _userService.Register(registerDto, Request.Scheme, Request.Host.ToString());
            return StatusCode(201, user);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var token = await _userService.Login(loginDto);
            return Ok(new { token });
        }

        [HttpGet("profiles")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetOne(string id)
        {
            var user = await _userService.GetUserById(id);
            return Ok(user);
        }

        [HttpPost("changeStatus/{id}")]
        public async Task<IActionResult> ChangeStatus(string id)
        {
            var isBlocked = await _userService.ChangeUserStatus(id);
            return Ok(new { message = isBlocked ? "User has been blocked." : "User has been unblocked." });
        }

        [HttpPost("editRole")]
        public async Task<IActionResult> EditRole(EditRoleDto editRoleDto)
        {
            var result = await _userService.EditUserRoles(editRoleDto);
            return Ok(result);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _userService.CreateRoles();
            return Ok(roles);
        }

        [HttpPost("update/{id}")]
        public async Task<IActionResult> Update(string id, UpdateUserDto updateUserDto)
        {
            var updatedUser = await _userService.UpdateUser(id, updateUserDto);
            return Ok(updatedUser);
        }

        [HttpGet("verifyEmail")]
        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            var result = await _userService.VerifyEmail(email, token);
            return result ? Redirect("https://localhost:7296/user/login") : BadRequest("Invalid token or confirmation failed.");
        }


        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var message = await _userService.ForgotPassword(email, Request.Scheme, Request.Host.ToString());
            return Ok(new { message });
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword(string email, string token, [FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
            {
                return BadRequest("Passwords do not match.");
            }

            var result = await _userService.ResetPassword(email, token, resetPasswordDto);
            return result ? Ok("Password has been reset successfully.") : BadRequest("Password reset failed.");
        }

    }
}
