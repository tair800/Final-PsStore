using Final.Mvc.ViewModels.ChatVMs;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Final.Mvc.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            var token = Request.Cookies["token"];
            string userName = null;
            string userId = null;
            bool isAuthenticated = !string.IsNullOrEmpty(token);

            if (isAuthenticated)
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                userName = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
            }

            var model = new ChatModel
            {
                UserName = userName,
                UserId = userId,
                IsAuthenticated = isAuthenticated,
                Messages = new List<ChatMessage>()
            };

            return View(model);
        }
    }
}
