using Final.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;

namespace Final.Mvc.Controllers
{
    public class SettingController : Controller
    {
        private readonly IEmailService _emailService;

        public SettingController(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public async Task<IActionResult> Index()
        {
            var token = Request.Cookies["token"];

            bool isAuthenticated = !string.IsNullOrEmpty(token);
            string userEmail = null;
            string userName = null;

            if (isAuthenticated)
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                userEmail = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                userName = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            }

            ViewBag.IsAuthenticated = isAuthenticated;
            ViewBag.UserEmail = userEmail;
            ViewBag.UserName = userName;

            // Fetch settings as before
            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.GetAsync("https://localhost:7047/api/setting");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
                return View(result);
            }

            return BadRequest("Error fetching settings");
        }


        [HttpPost]
        public async Task<IActionResult> SendSupportEmail(string name, string email, string subject, string message)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(message))
            {
                TempData["Error"] = "All fields are required.";
                return RedirectToAction("Index");
            }

            // Encode inputs to prevent injection attacks
            var encodedName = WebUtility.HtmlEncode(name);
            var encodedEmail = WebUtility.HtmlEncode(email);
            var encodedSubject = WebUtility.HtmlEncode(subject);
            var encodedMessage = WebUtility.HtmlEncode(message);

            // Load the email template
            string emailBodyTemplate;
            try
            {
                using (StreamReader sr = new StreamReader("wwwroot/templates/emailTemplate/supportEmail.html"))
                {
                    emailBodyTemplate = await sr.ReadToEndAsync();
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Could not load email template. Please try again later.";
                return RedirectToAction("Index");
            }

            // Replace placeholders with actual values
            string emailBody = emailBodyTemplate
                .Replace("{{Name}}", encodedName)
                .Replace("{{Email}}", encodedEmail)
                .Replace("{{Subject}}", encodedSubject)
                .Replace("{{Message}}", encodedMessage);

            try
            {
                // Send the email using the email service
                _emailService.SendEmail(
                    from: "tahiraa@code.edu.az",
                    to: "tahiraa@code.edu.az",
                    subject: $"Support Request from {encodedName}: {encodedSubject}",
                    body: emailBody,
                    smtpHost: "smtp.gmail.com",
                    smtpPort: 587,
                    enableSsl: true,
                    smtpUser: "tahiraa@code.edu.az",
                    smtpPass: "blcf yubd mxnb gcyb"
                );

                TempData["Success"] = "Your message has been sent successfully. We will get back to you shortly.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while sending your message: {ex.Message}";
            }

            return RedirectToAction("Index");
        }



    }
}
