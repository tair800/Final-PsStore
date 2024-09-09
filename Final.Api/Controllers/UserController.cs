using AutoMapper;
using Final.Application.Dtos.UserDtos;
using Final.Application.Services.Interfaces;
using Final.Application.Settings;
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
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _jwtSettings;

        public UserController(IOptions<JwtSettings> jwtSettings, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IMapper mapper, ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var existUser = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existUser != null) return Conflict(new { message = "Username already exists." });

            var user = _mapper.Map<User>(registerDto);

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, "member");

            return StatusCode(201);

        }

        //[HttpGet]
        //public async Task<IActionResult> CreateRole()
        //{
        //    if (!await _roleManager.RoleExistsAsync("member"))
        //        await _roleManager.CreateAsync(new IdentityRole() { Name = "member" });

        //    if (!await _roleManager.RoleExistsAsync("admin"))
        //        await _roleManager.CreateAsync(new IdentityRole() { Name = "admin" });

        //    if (!await _roleManager.RoleExistsAsync("superAdmin"))
        //        await _roleManager.CreateAsync(new IdentityRole() { Name = "superAdmin" });

        //    return StatusCode(201);
        //}

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            //var user = await _userManager.FindByEmailAsync(loginDto.Email);

            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user is null) return BadRequest("Username or Email is wrong");

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result) return BadRequest();

            //jwt
            var roles = await _userManager.GetRolesAsync(user);
            var secretKet = _jwtSettings.SecretKey;
            var audience = _jwtSettings.Audience;
            var issuer = _jwtSettings.Issuer;

            return Ok(new { token = _tokenService.GetToken(secretKet, audience, issuer, user, roles) });
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

        [HttpGet("profile/{name}")]
        public async Task<IActionResult> GetOne(string name)
        {
            var user = await _userManager.FindByNameAsync(name);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userDto = _mapper.Map<UserReturnDto>(user);
            userDto.UserRoles = roles.ToList();

            return Ok(userDto);
        }


    }
}
