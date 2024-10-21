using KafeHaruno.Entities;
using KafeHaruno.KafeHarunoDbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KafeHaruno.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext context;

        public UserController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            var userEmail = HttpContext.User.FindFirst("UserName")?.Value; // Get the user's email address
            var User = context.Users.FirstOrDefault(u => u.Username == userEmail); // Get the user

            // If the user is not found, you can provide an appropriate error message or redirect
            if (User == null)
            {
                return NotFound("User not found.");
            }

            return View( User ); // List only this user
        }

        [Authorize]
        public IActionResult SecurePage()
        {
            var userName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            ViewBag.Name = userName;

            var userRole = HttpContext.User.FindFirst("Role")?.Value; // Get the user role
            ViewBag.Role = userRole;

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await context.Users.FindAsync(id);

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User viewModel)
        {

            // Find user in Database
            var user = await context.Users.FindAsync(viewModel.Id);
            if (user != null)
            {
                // Update users' information
                user.Name = viewModel.Name;
                user.Surname = viewModel.Surname;
                user.Username = viewModel.Username;
                user.Password = viewModel.Password;
                user.Role = viewModel.Role;

                // save changes
                await context.SaveChangesAsync();

                // set the success message
                TempData["SuccessMessage"] = "User updated successfully.";

            }
            else
            {
                // Set error message if user not found
                TempData["ErrorMessage"] = "User not found.";
            }

            // Admin sayfasına yönlendir
            return RedirectToAction("Index", "User");
        }
    }
}
