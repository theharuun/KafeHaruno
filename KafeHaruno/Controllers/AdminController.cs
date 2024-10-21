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
        { // passaword silinecek ındexcshtmlden
            return View(context.Users.ToList());
        }



        [Authorize]
        public IActionResult SecurePage()
        {
            var userName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            ViewBag.Name = userName;

            var userRole = HttpContext.User.FindFirst("Role")?.Value; // Kullanıcı rolünü al
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

            // Kullanıcıyı veritabanında bul
            var user = await context.Users.FindAsync(viewModel.Id);
            if (user != null)
            {
                // Kullanıcı bilgilerini güncelle
                user.Name = viewModel.Name;
                user.Surname = viewModel.Surname;
                user.Username = viewModel.Username;
                user.Role = viewModel.Role;

                // Değişiklikleri kaydet
                await context.SaveChangesAsync();

                // Başarı mesajını ayarla
                TempData["SuccessMessage"] = "User updated successfully.";

            }
            else
            {
                // Kullanıcı bulunamadıysa hata mesajı ayarla
                TempData["ErrorMessage"] = "User not found.";
            }

            // Admin sayfasına yönlendir
            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            // Kullanıcıyı ve ilgili apartman bilgilerini al
            var user = await context.Users
                .Include(a => a.Orders)
                    .ThenInclude(o => o.OrderProducts) // Include OrderProducts
                        .ThenInclude(op => op.Product)  // Include Product for each OrderProduct
                .Include(a => a.Orders) // Re-include Orders to access Tables
                    .ThenInclude(o => o.Tables) // Include Tables for each Order
                .FirstOrDefaultAsync(u => u.Id == id);


            if (user == null)
            {
                return NotFound(); // Kullanıcı bulunamazsa 404 döndür
            }

            return View(user); // Detayları görüntülemek için view'ı döndür
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
