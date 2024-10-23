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
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string requestScheme = HttpContext.Request.Scheme; // Get the scheme (http/https)
            string requestHost = HttpContext.Request.Host.ToString(); // Get the host (e.g., localhost:5000)

            var result = await _userService.RegisterUserAsync(registerDto, requestScheme, requestHost);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return BadRequest(ModelState);
            }

            return Ok(new { message = "Registration successful, please verify your email." });
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


        [HttpGet("verifyemail")]
        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            await _userService.VerifyEmailAsync(email, token);
            return Redirect("https://localhost:7296/user/login");
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

        //[HttpPost("confirmEmail")]
        //public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto model)
        //{
        //    try
        //    {
        //        await _userService.ConfirmEmail(model.Email, model.Token);
        //        return Ok(new { Message = "Email confirmed successfully." });
        //    }
        //    catch (CustomExceptions ex)
        //    {
        //        return StatusCode(ex.Code, new { Message = ex.Message, Errors = ex.Errors });
        //    }
        //}


        [HttpGet("verified")]
        public async Task<IActionResult> GetVerifiedUsers()
        {
            try
            {
                var verifiedUsers = await _userService.GetVerifiedUsersAsync();
                return Ok(verifiedUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("adminPasswordChange/{userId}")]
        public async Task<IActionResult> AdminPasswordChange(string userId, AdminPasswordChangeDto adminPasswordChangeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.AdminPasswordChange(userId, adminPasswordChangeDto.Password);
            if (!result)
                return BadRequest("Failed to change password.");

            return Ok("Password changed successfully.");
        }
        [HttpPost("createRole")]
        public async Task<IActionResult> CreateRole()
        {
            var result = await _userService.CreateRoles();
            if (result)
            {
                return Ok(new { message = "Roles created successfully." });
            }

            return BadRequest("Failed to create roles.");
        }

        [HttpGet("{userId}/cards")]
        public async Task<IActionResult> GetUserCards(string userId)
        {
            // Retrieve user's saved cards from the database
            var cards = await _userService.GetUserCards(userId);

            if (cards == null || !cards.Any())
            {
                return NotFound("No cards found.");
            }

            return Ok(cards);
        }

        [HttpPost("{userId}/cards")]
        public async Task<IActionResult> SaveUserCard(string userId, [FromBody] SaveCardDto cardDto)
        {
            // Validate and save the user's card details
            var result = await _userService.SaveCard(userId, cardDto);

            if (result)
            {
                return Ok("Card saved successfully.");
            }

            return BadRequest("Failed to save card.");
        }

        [HttpDelete("{userId}/cards/{cardId}")]
        public async Task<IActionResult> DeleteCard(string userId, int cardId)
        {
            try
            {
                // Call service method to delete the card
                var result = await _userService.DeleteCard(userId, cardId);
                if (result)
                {
                    // Return success response with a custom message
                    return Ok(new { Message = "Card deleted successfully." });
                }
                else
                {
                    return BadRequest(new { Message = "Failed to delete card." });
                }
            }
            catch (CustomExceptions ex)
            {
                // Return an error response in case of a custom exception
                return StatusCode(ex.Code, new { Message = ex.Message, Errors = ex.Errors });
            }
            catch (Exception ex)
            {
                // Return a generic error response for unhandled exceptions
                return StatusCode(500, new { Message = "An error occurred while deleting the card.", Details = ex.Message });
            }
        }


    }
}
