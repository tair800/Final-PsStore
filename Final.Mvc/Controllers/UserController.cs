using Final.Application.Dtos.UserDtos;
using Final.Application.Services.Interfaces;
using Final.Mvc.ViewModels.UserVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;

namespace Final.Mvc.Controllers
{
    public class UserController : Controller
    {
        private readonly IEmailService emailService;
        private readonly HttpClient _httpClient;


        public UserController(IEmailService emailService, HttpClient httpClient)
        {
            this.emailService = emailService;
            _httpClient = httpClient;
        }

        public IActionResult Login()
        {

            var token = Request.Cookies["token"];
            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Update", "User");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            using HttpClient client = new();
            StringContent content = new(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://localhost:7047/api/user/login", content);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<UserToken>(data);

                // Store the token securely
                Response.Cookies.Append("token", result.Token, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.Now.AddHours(1)
                });

                return RedirectToAction("Index", "Home");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                // Safely parse the error content to extract only the message and validation errors
                try
                {
                    // Deserialize error content into a structured object
                    var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);

                    // General error message
                    if (!string.IsNullOrEmpty(errorResponse.Message))
                    {
                        ViewBag.ErrorMessage = errorResponse.Message;
                    }

                    // Field-specific validation errors
                    if (errorResponse.Errors != null)
                    {
                        foreach (var error in errorResponse.Errors)
                        {
                            foreach (var errorMessage in error.Value)
                            {
                                ModelState.AddModelError(error.Key, errorMessage); // Add error to the specific field
                            }
                        }
                    }
                }
                catch (JsonException ex)
                {
                    // If parsing fails, show a generic error
                    Console.WriteLine($"Error parsing JSON: {ex.Message}");
                    ViewBag.ErrorMessage = "An unexpected error occurred during login.";
                }

                return View(model); // Return the view with the error messages
            }
        }


        public IActionResult Register()
        {
            return View(new RegisterVM());
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            // Create the request body
            var jsonContent = JsonConvert.SerializeObject(registerVM);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Send the request to the API
            var response = await _httpClient.PostAsync("https://localhost:7047/api/user/register", content);

            if (response.IsSuccessStatusCode)
            {
                // Handle success (e.g., redirect to a success page or show a success message)
                return RedirectToAction("Index", "Home");
            }

            // Handle error (e.g., return the error messages to the view)
            var errorResponse = await response.Content.ReadAsStringAsync();

            // Safely parse the error content to extract only the message and validation errors
            try
            {
                // Deserialize error content into a structured object
                var error = JsonConvert.DeserializeObject<ErrorResponse>(errorResponse);

                // General error message
                if (!string.IsNullOrEmpty(error.Message))
                {
                    ViewBag.ErrorMessage = error.Message;
                }

                // Field-specific validation errors
                if (error.Errors != null)
                {
                    foreach (var err in error.Errors)
                    {
                        foreach (var errorMessage in err.Value)
                        {
                            ModelState.AddModelError(err.Key, errorMessage); // Add error to the specific field
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                // If parsing fails, show a generic error
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
                ViewBag.ErrorMessage = "An unexpected error occurred during registration.";
            }

            return View(registerVM); // Return the view with the error messages
        }


        public IActionResult Logout()
        {
            Response.Cookies.Delete("token");
            return RedirectToAction("Login");
        }



        // Forgot Password View
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            var resetPasswordEmailDto = new ForgotPasswordEmailVM
            {
                email = model.Email,
                token = model.Token,
            };

            // Corrected to serialize the entire object
            var jsonContent = JsonConvert.SerializeObject(resetPasswordEmailDto);
            HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            Console.WriteLine(content.ToString());
            using var client = new HttpClient();

            var apiUrl = "https://localhost:7047/api/User/forgotPassword";
            var response = await client.PostAsync(apiUrl, content);
            Console.WriteLine(response.StatusCode);
            var responseContent2 = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent2);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ResetPasswordEmailVM>(responseContent);
                var email = apiResponse.Message.Email;
                var token = apiResponse.Message.Token;
                var resetLink = Url.Action(
                "ResetPassword",
                "User",
                new { email = email, token = token },
                protocol: HttpContext.Request.Scheme);
                string body;
                using (StreamReader sr = new StreamReader("wwwroot/templates/passwordTemplate/forgotPassword.html"))
                {
                    body = sr.ReadToEnd();
                }
                body = body.Replace("{{link}}", resetLink).Replace("{{UserName}}", email);
                emailService.SendEmail(
                from: "tahiraa@code.edu.az",
                to: email,
                subject: "ResetPassword",
                body: body,
                smtpHost: "smtp.gmail.com",
                smtpPort: 587,
                enableSsl: true,
                smtpUser: "tahiraa@code.edu.az",
                smtpPass: "cark zrzn cjid cjlr"
            );

                TempData["EmailSendingSuccess"] = "An email with instructions has been sent to the provided address.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var errorResponseString = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(errorResponseString);

                if (errorResponse?.Errors != null)
                {
                    foreach (var error in errorResponse.Errors)
                    {
                        ModelState.AddModelError(error.Key, error.Value);
                        TempData["ForgetPasswordError"] = $"{error.Key}: {error.Value}";
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorResponse?.Message ?? "An unknown error occurred.");
                }

                return View(model);
            }
        }

        // Reset Password View
        public IActionResult ResetPassword(string email, string token)
        {
            var model = new ResetPasswordVM { Email = email, Token = token };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            using HttpClient client = new();
            StringContent content = new(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://localhost:7047/api/user/resetPassword", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Your password has been reset successfully.";
                return RedirectToAction("Login");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                try
                {
                    var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                    if (!string.IsNullOrEmpty(errorResponse?.Message))
                    {
                        ViewBag.ErrorMessage = errorResponse.Message;
                    }
                }
                catch (JsonException)
                {
                    ViewBag.ErrorMessage = "An unexpected error occurred while resetting the password.";
                }

                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            using HttpClient client = new();
            var confirmationData = new { email, token };
            StringContent content = new(JsonConvert.SerializeObject(confirmationData), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://localhost:7047/api/User/verifyEmail", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["ConfirmationSuccess"] = "Email confirmed successfully. You can now log in.";
                return RedirectToAction("Login");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["ConfirmationError"] = $"Email confirmation failed: {errorContent}";
                return RedirectToAction("Error");
            }
        }


        public IActionResult Update()
        {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var userId = GetUserIdFromToken(token); // Extract the userId from the token
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var response = client.GetAsync($"https://localhost:7047/api/user/profile/{userId}").Result;

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine($"Error: {errorContent}");
                return RedirectToAction("Login");
            }

            var userContent = response.Content.ReadAsStringAsync().Result;
            var user = JsonConvert.DeserializeObject<UpdateUserVM>(userContent);

            // Ensure UserId is populated in the model
            user.UserId = userId;

            // Make sure email cannot be modified
            ViewBag.Roles = GetAvailableRoles(); // Retrieve available roles from the server or predefined list
            return View(user);
        }


        [HttpPost]
        public async Task<IActionResult> Update(UpdateUserVM model)
        {

            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            // Extract UserId from the token
            var userId = GetUserIdFromToken(token);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }



            // Update the UserId in the model if it is null
            model.UserId ??= userId;

            // Map UpdateUserVM to UpdateUserDto
            var updateUserDto = new UpdateUserDto
            {
                UserId = model.UserId,
                UserName = model.UserName,
                FullName = model.FullName,
                PasswordConfirmation = model.CurrentPassword
            };

            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            StringContent content = new(JsonConvert.SerializeObject(updateUserDto), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync($"https://localhost:7047/api/user/update/{model.UserId}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["UpdateSuccess"] = "User information updated successfully.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Failed to update user: {errorContent}");

                // Retrieve available roles again to repopulate dropdown
                ViewBag.Roles = GetAvailableRoles();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> MyCard()
        {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            string userId = GetUserIdFromToken(token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _httpClient.GetAsync($"https://localhost:7047/api/User/{userId}/cards");

            var model = new UserCardsVM();

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                model.SavedCards = JsonConvert.DeserializeObject<List<UserCardsVM.SavedCardVM>>(data);
            }

            return View(model);
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteCard(string cardId)
        {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            string userId = GetUserIdFromToken(token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _httpClient.DeleteAsync($"https://localhost:7047/api/User/{userId}/cards/{cardId}");

            if (response.IsSuccessStatusCode)
            {
                return Ok(new { Message = "Card deleted successfully." });
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return BadRequest($"Failed to delete card: {errorContent}");
        }


        private List<string> GetAvailableRoles()
        {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return new List<string>();
            }

            // Encode the token for secure transmission
            var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));

            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {encodedToken}");

            var response = client.GetAsync("https://localhost:7047/api/user/roles").Result;

            if (!response.IsSuccessStatusCode)
            {
                // Log or handle error appropriately
                return new List<string>();
            }

            var rolesContent = response.Content.ReadAsStringAsync().Result;
            var roles = JsonConvert.DeserializeObject<List<string>>(rolesContent);

            return roles ?? new List<string>();
        }

        private string GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
        }

        public class UserToken
        {
            public string Token { get; set; }
        }
    }
}
