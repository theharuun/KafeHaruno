using KafeHaruno.Entities;
using KafeHaruno.KafeHarunoDbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            var userEmail = HttpContext.User.FindFirst("UserName")?.Value; // Kullanıcının e-posta adresini al
            var User = context.Users.FirstOrDefault(u => u.Username == userEmail); // Kullanıcıyı al

            // Kullanıcı bulunamazsa, uygun bir hata mesajı veya yönlendirme yapabilirsiniz
            if (User == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            return View( User ); // Sadece bu kullanıcıyı listele
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
        public async Task<IActionResult> Edit(int id)
        {
            var user = await context.Users.FindAsync(id);

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User viewModel)
        {

            // Kullanıcıyı veritabanında bul
            var user = await context.Users.FindAsync(viewModel.Id);
            if (user != null)
            {
                // Kullanıcı bilgilerini güncelle
                user.Name = viewModel.Name;
                user.Surname = viewModel.Surname;
                user.Username = viewModel.Username;
                user.Password = viewModel.Password;
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
            return RedirectToAction("Index", "User");
        }
    }
}
