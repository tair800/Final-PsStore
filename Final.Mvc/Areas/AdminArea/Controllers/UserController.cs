using Final.Application.Dtos.UserDtos;
using Final.Mvc.Areas.AdminArea.ViewModels.UserVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace YourProjectNamespace.Areas.Admin.Controllers
{
    [Area("AdminArea")]
    public class UserController : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<UserListVM> users = new List<UserListVM>();

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            HttpResponseMessage response = await client.GetAsync("https://localhost:7047/api/user/profiles");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                users = JsonConvert.DeserializeObject<List<UserListVM>>(data);
            }
            else
            {
                ViewBag.ErrorMessage = "Unable to retrieve user data.";
            }

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(string id)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            HttpResponseMessage response = await client.PostAsync($"https://localhost:7047/api/user/changeStatus/{id}", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "User status has been changed successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to change the user's status.";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            UserReturnVM user;

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            HttpResponseMessage response = await client.GetAsync($"https://localhost:7047/api/user/profile/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<UserReturnVM>(data);
                return View(user);
            }
            else
            {
                return NotFound("User not found.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User ID cannot be null.");
            }

            RoleVM roleVM = new RoleVM();  // Initialize the view model

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            // Fetch user profile (which includes their roles)
            HttpResponseMessage userResponse = await client.GetAsync($"https://localhost:7047/api/user/profile/{id}");

            if (userResponse.IsSuccessStatusCode)
            {
                var userData = await userResponse.Content.ReadAsStringAsync();
                roleVM = JsonConvert.DeserializeObject<RoleVM>(userData);

                // Ensure the UserId is set correctly
                roleVM.UserId = id;

                // Fetch all available roles to display for selection
                HttpResponseMessage rolesResponse = await client.GetAsync($"https://localhost:7047/api/user/roles");
                if (rolesResponse.IsSuccessStatusCode)
                {
                    var rolesData = await rolesResponse.Content.ReadAsStringAsync();
                    roleVM.AvailableRoles = JsonConvert.DeserializeObject<List<string>>(rolesData);
                }

                return View(roleVM);
            }
            else
            {
                return NotFound("User not found.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(RoleVM model, string[] SelectedRoles)
        {
            if (SelectedRoles == null || !SelectedRoles.Any())
            {
                // Fetch the available roles again since it's not retained when returning the view
                using HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

                HttpResponseMessage rolesResponse = await client.GetAsync($"https://localhost:7047/api/user/roles");
                if (rolesResponse.IsSuccessStatusCode)
                {
                    var rolesData = await rolesResponse.Content.ReadAsStringAsync();
                    model.AvailableRoles = JsonConvert.DeserializeObject<List<string>>(rolesData);
                }

                TempData["ErrorMessage"] = "At least one role must be selected.";
                return View(model);
            }

            using (HttpClient client = new())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

                // Create a new object to send the selected roles to the API
                var editRoleDto = new
                {
                    UserId = model.UserId,
                    Roles = SelectedRoles
                };

                var content = new StringContent(JsonConvert.SerializeObject(editRoleDto), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync($"https://localhost:7047/api/user/editRole", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "User roles have been updated successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update the user's roles.";
                    return View(model);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Id cannot be null or empty.");

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            HttpResponseMessage response = await client.GetAsync($"https://localhost:7047/api/user/profile/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<UserUpdateVM>(data);

                return View(user);
            }

            TempData["ErrorMessage"] = "Unable to retrieve user data.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update(string id, UserUpdateVM model)
        {
            if (string.IsNullOrEmpty(model.PasswordConfirmation))
            {
                TempData["ErrorMessage"] = "Password confirmation is required.";
                return View(model);
            }

            var updateUserDto = new UpdateUserDto
            {
                UserName = model.UserName,
                FullName = model.FullName,
                PasswordConfirmation = model.PasswordConfirmation
            };

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            var content = new StringContent(JsonConvert.SerializeObject(updateUserDto), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync($"https://localhost:7047/api/user/update/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "User information has been updated successfully.";
                return RedirectToAction("Index");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                TempData["ErrorMessage"] = "Username is already taken.";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                TempData["ErrorMessage"] = "Password confirmation is incorrect.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update user information.";
            }

            return View(model);
        }
    }
}
