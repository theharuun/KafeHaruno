using KafeHaruno.KafeHarunoDbContext;
using KafeHaruno.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace KafeHaruno.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = context.Users.Where(s => s.Username == model.Username && s.Password == model.Password ).FirstOrDefault();
                if (user != null)
                {
                    //success
                    var claims = new List<Claim>
                    {
                        new Claim("UserName"  ,user.Username),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Add user ID
                        new Claim("Role", user.Role == true ? "Admin" : user.Role == false ? "Waiter/Waitress" : "Guest") // Role based on Role value
                    };



                    var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity));

                    // Routing based on user role
                    if (user.Role is false)
                    {
                        return RedirectToAction("SecurePage", "User"); // If the user role is redirected to UserController
                    }
                    else
                    {
                        return RedirectToAction("SecurePage", "Admin"); // Redirect to AdminController for other roles
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Email or Password is not correct .");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
