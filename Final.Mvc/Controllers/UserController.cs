using Final.Application.Dtos.UserDtos;
using Final.Application.Services.Interfaces;
using Final.Mvc.ViewModels.UserVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Final.Mvc.Controllers
{
    public class UserController : Controller
    {
        private readonly IEmailService emailService;


        public UserController(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        public IActionResult Login()
        {
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
                    HttpOnly = false, // Ensure the token is only accessible via HTTP(S)
                    Secure = true,   // Secure the cookie (HTTPS)
                    SameSite = SameSiteMode.Strict, // Ensure the token is sent with same-site requests only
                    Expires = DateTimeOffset.Now.AddHours(1) // Optional: set an expiration time
                });

                return RedirectToAction("Index", "Home");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = $"Login failed: {errorContent}";
            }

            return View();
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

            using HttpClient client = new();
            StringContent content = new StringContent(JsonConvert.SerializeObject(registerVM), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://localhost:7047/api/user/register", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<RegisterResponseVM>(responseContent);

                var email = apiResponse.Email;
                var token = apiResponse.Token;

                var confirmLink = Url.Action(
                    "ConfirmEmail",
                    "User",
                    new { email = email, token = token },
                    protocol: HttpContext.Request.Scheme);

                string body;
                using (StreamReader sr = new StreamReader("wwwroot/templates/emailTemplate/emailConfirm.html"))
                {
                    body = sr.ReadToEnd();
                }

                body = body.Replace("{{link}}", confirmLink).Replace("{{UserName}}", email);

                // Send confirmation email
                emailService.SendEmail(
                    from: "tahiraa@code.edu.az",
                    to: email,
                    subject: "Confirm Your Registration",
                    body: body,
                    smtpHost: "smtp.gmail.com",
                    smtpPort: 587,
                    enableSsl: true,
                    smtpUser: "tahiraa@code.edu.az",
                    smtpPass: "blcf yubd mxnb gcyb"
                );

                TempData["RegistrationSuccess"] = "Registration successful. Please check your email to confirm your account.";
                return RedirectToAction("Login");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Registration failed: {errorContent}");
                return View(registerVM);
            }
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
                subject: "Verify Email",
                body: body,
                smtpHost: "smtp.gmail.com",
                smtpPort: 587,
                enableSsl: true,
                smtpUser: "tahiraa@code.edu.az",
                smtpPass: "blcf yubd mxnb gcyb"
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using HttpClient client = new();
            StringContent content = new(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://localhost:7047/api/user/resetPassword", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Failed to reset password: {errorContent}");
            }

            return View();
        }

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
