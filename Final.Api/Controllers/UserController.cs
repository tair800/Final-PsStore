using AutoMapper;
using Final.Application.Dtos.UserDtos;
using Final.Application.Exceptions;
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
            var userReturnDto = await _userService.Register(registerDto, Request.Scheme, Request.Host.ToString());
            var userViewModel = _mapper.Map<UserReturnDto>(userReturnDto);
            return StatusCode(201, userViewModel);
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

        [HttpGet("profile/{userId}")]
        public async Task<IActionResult> GetOne(string userId)
        {
            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }
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
            var roles = await _userService.GetAllRoles();
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
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var result = await _userService.ForgotPassword(forgotPasswordDto);
            return Ok(new { message = result });
        }


        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var result = await _userService.ResetPassword(resetPasswordDto);
            return result ? Ok("Password has been reset successfully.") : BadRequest("Failed to reset password.");
        }

        [HttpPost("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto model)
        {
            try
            {
                await _userService.ConfirmEmail(model.Email, model.Token);
                return Ok(new { Message = "Email confirmed successfully." });
            }
            catch (CustomExceptions ex)
            {
                return StatusCode(ex.Code, new { Message = ex.Message, Errors = ex.Errors });
            }
        }

    }
}
