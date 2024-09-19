using AutoMapper;
using Final.Application.Dtos.UserDtos;
using Final.Application.JwtSettings;
using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Final.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        private readonly JwtSettings _jwtSettings;

        public UserController(IOptions<JwtSettings> jwtSettings, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IMapper mapper, ITokenService tokenService, SignInManager<User> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {


            var existUser = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existUser != null)
            {
                return Conflict(new { message = "Username already exists." });
            }

            var user = _mapper.Map<User>(registerDto);

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string link = Url.Action(nameof(VerifyEmail), "Account", new { email = user.Email, token },
                Request.Scheme, Request.Host.ToString());

            string body = string.Empty;
            using (StreamReader reader = new StreamReader("wwwroot/templates/emailTemplate/emailConfirm.html"))
            {
                body = reader.ReadToEnd();
            };
            body = body.Replace("{{link}}", link);
            body = body.Replace("{{username}}", user.FullName);
            _emailService.SendEmail(new() { user.Email }, body, "Email verification", "Verify email");

            await _userManager.AddToRoleAsync(user, "member");

            return StatusCode(201);


        }




        [HttpGet("Role")]
        public async Task<IActionResult> CreateRole()
        {
            if (!await _roleManager.RoleExistsAsync("member"))
                await _roleManager.CreateAsync(new IdentityRole() { Name = "member" });

            if (!await _roleManager.RoleExistsAsync("admin"))
                await _roleManager.CreateAsync(new IdentityRole() { Name = "admin" });

            if (!await _roleManager.RoleExistsAsync("superAdmin"))
                await _roleManager.CreateAsync(new IdentityRole() { Name = "superAdmin" });

            return StatusCode(201);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user == null)
                return BadRequest("Invalid username.");

            if (user.IsBlocked)
                return Forbid("Your account has been blocked. Please contact support.");

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result)
                return BadRequest("Invalid password.");

            var roles = await _userManager.GetRolesAsync(user);

            var secretKey = _jwtSettings.SecretKey;
            var audience = _jwtSettings.Audience;
            var issuer = _jwtSettings.Issuer;

            var token = _tokenService.GetToken(secretKey, audience, issuer, user, roles);

            return Ok(new { token });
        }




        [HttpGet("profiles")]
        public async Task<IActionResult> GetAll()
        {

            var users = _userManager.Users.ToList();
            var userDtos = new List<UserReturnDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userDto = _mapper.Map<UserReturnDto>(user);
                userDto.UserRoles = roles.ToList();

                userDtos.Add(userDto);
            }

            return Ok(userDtos);
        }



        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetOne(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userDto = _mapper.Map<UserReturnDto>(user);
            userDto.UserRoles = roles.ToList();

            return Ok(userDto);
        }





        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                return BadRequest("Email not found.");
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = Url.Action(nameof(ResetPassword), "User"
                , new { email = user.Email, token = resetToken }
                , Request.Scheme
                , Request.Host.ToString());

            string body = string.Empty;
            using (StreamReader reader = new StreamReader("wwwroot/templates/passwordTemplate/forgotPassword.html"))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{{link}}", resetLink);
            body = body.Replace("{{username}}", user.FullName);

            _emailService.SendEmail(new() { user.Email }, body, "Password Reset", "Reset your password");

            return Ok("Password reset link has been sent to your email.");
        }


        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return BadRequest("Invalid request.");
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Password has been reset successfully.");
        }


        [HttpPost("verifyEmail")]
        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            User user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return BadRequest("Invalid email.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest("Invalid token or confirmation failed.");
            }

            await _signInManager.SignInAsync(user, true);

            return Ok(new { message = "Email verified successfully. You can now log in.", redirectUrl = "https://yourapp.com/login" });
        }

        [HttpPost("changeStatus/{id}")]
        public async Task<IActionResult> ChangeStatus(string id)
        {
            // Ensure the ID is provided
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            // Find the user by their ID
            var user = await _userManager.FindByIdAsync(id);

            // If user does not exist, return a bad request
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Toggle the IsBlocked status
            user.IsBlocked = !user.IsBlocked;

            // Update the user's status
            var result = await _userManager.UpdateAsync(user);

            // Check if the update was successful
            if (!result.Succeeded)
            {
                return BadRequest("Unable to change the user's status.");
            }

            // Return success response
            return Ok(new { message = user.IsBlocked ? "User has been blocked." : "User has been unblocked." });
        }




    }
}
