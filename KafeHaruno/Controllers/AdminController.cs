using KafeHaruno.Entities;
using KafeHaruno.KafeHarunoDbContext;
using KafeHaruno.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KafeHaruno.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext context;

        public AdminController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        { 
            return View(context.Users.ToList());
        }



        [Authorize]
        public IActionResult SecurePage()
        {
            var userName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            ViewBag.Name = userName;

            var userRole = HttpContext.User.FindFirst("Role")?.Value; // Get user role
            ViewBag.Role = userRole;

            return View();
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserViewModel viewModel)
        {
            var user = new User
            {
                Name = viewModel.Name,
                Surname = viewModel.Surname,
                Username=viewModel.Username,
                Password = viewModel.Password,
                Role = viewModel.Role
            };

            await context.Users.AddAsync(user);

            await context.SaveChangesAsync();

            return RedirectToAction("Index","Admin");
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await context.Users.FindAsync(id);

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(User viewModel)
        {

            // Find user in database
            var user = await context.Users.FindAsync(viewModel.Id);
            if (user != null)
            {
                // Update user information
                user.Name = viewModel.Name;
                user.Surname = viewModel.Surname;
                user.Username = viewModel.Username;
                user.Role = viewModel.Role;

                // Save changes
                await context.SaveChangesAsync();

                // Set success message
                TempData["SuccessMessage"] = "User updated successfully.";

            }
            else
            {
                // Set error message if user not found
                TempData["ErrorMessage"] = "User not found.";
            }

            // Redirect to admin page
            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            // Get user and related apartment information
            var user = await context.Users
                .Include(a => a.Orders)
                    .ThenInclude(o => o.OrderProducts) // Include OrderProducts
                        .ThenInclude(op => op.Product)  // Include Product for each OrderProduct
                .Include(a => a.Orders) // Re-include Orders to access Tables
                    .ThenInclude(o => o.Tables) // Include Tables for each Order
                .FirstOrDefaultAsync(u => u.Id == id);


            if (user == null)
            {
                return NotFound(); // Return 404 if user not found
            }

            return View(user); // Rotate view to view details
        }

        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await context.Users.FindAsync(id);

            if (user == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            context.Users.Remove(user);
            await context.SaveChangesAsync();

            return RedirectToAction("Index", "Admin");

        }

    }
}
