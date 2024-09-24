using AutoMapper;
using Final.Application.Dtos.UserDtos;
using Final.Application.JwtSettings;
using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            //if (!IsValidEmail(registerDto.Email))
            //{
            //    return BadRequest("Invalid email address.");
            //}

            var existUser = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existUser != null)
            {
                return Conflict(new { message = "Username already exists." });
            }

            var user = _mapper.Map<User>(registerDto);

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, "member");


            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string link = Url.Action(nameof(VerifyEmail), "User", new { email = user.Email, token },
                Request.Scheme, Request.Host.ToString());

            string body = string.Empty;
            using (StreamReader reader = new StreamReader("wwwroot/templates/emailTemplate/emailConfirm.html"))
            {
                body = reader.ReadToEnd();
            };
            body = body.Replace("{{link}}", link).Replace("{{UserName}}", user.FullName);
            _emailService.SendEmail(new() { user.Email }, body, "Email verification", "Verify email");


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
            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }

            var userDtos = new List<UserReturnDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userDto = _mapper.Map<UserReturnDto>(user);

                // Ensure roles are included in the UserReturnDto
                userDto.UserRoles = roles.ToList();

                userDtos.Add(userDto);
            }

            return Ok(userDtos);
        }

        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetOne(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User ID cannot be null.");
            }

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

        [HttpGet("verifyEmail")]
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

            //await _signInManager.SignInAsync(user, true);

            return Redirect("https://localhost:7296/user/login");
        }

        [HttpPost("changeStatus/{id}")]
        public async Task<IActionResult> ChangeStatus(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("User ID cannot be null or empty.");


            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound("User not found.");

            user.IsBlocked = !user.IsBlocked;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest("Unable to change the user's status.");


            return Ok(new { message = user.IsBlocked ? "User has been blocked." : "User has been unblocked." });
        }

        [HttpPost("editRole")]
        public async Task<IActionResult> EditRole(EditRoleDto editRoleDto)
        {
            var user = await _userManager.FindByIdAsync(editRoleDto.UserId);

            if (user == null)
                return NotFound("User not found.");

            // Remove the user's current roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!result.Succeeded)
                return BadRequest("Failed to remove old roles.");

            // Assign the newly selected roles
            result = await _userManager.AddToRolesAsync(user, editRoleDto.Roles);

            if (!result.Succeeded)
                return BadRequest("Failed to add new roles.");

            return Ok();
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();

            if (roles == null || !roles.Any())
            {
                return NotFound("No roles found.");
            }

            return Ok(roles);
        }

        [HttpPost("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Id cannot be null or empty.");

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound("User not found.");

            var passwordCheck = await _userManager.CheckPasswordAsync(user, updateUserDto.PasswordConfirmation);
            if (!passwordCheck)
                return Unauthorized(" Confirm password is incorrect.");

            if (await _userManager.Users.AnyAsync(u => u.UserName == updateUserDto.UserName && u.Id != user.Id))
                return Conflict("Username is already taken.");

            _mapper.Map(updateUserDto, user);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var updatedUserDto = _mapper.Map<UserReturnDto>(user);

            return Ok(updatedUserDto);
        }

    }
}
