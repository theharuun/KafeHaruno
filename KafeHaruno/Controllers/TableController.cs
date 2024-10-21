using KafeHaruno.KafeHarunoDbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KafeHaruno.Controllers
{
    public class TableController : Controller
    {
        private readonly ApplicationDbContext context;

        public TableController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AdminIndex()
        {
            var tables = await context.Tables.Include(b=>b.Bills).ToListAsync();
            return View(tables);
        }

        public async Task<IActionResult> UserIndex()
        {
            var tables = await context.Tables.Include(b => b.Bills).Include(a=>a.Orders).ToListAsync();
            return View(tables);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var table = await context.Tables
                .Include(b=>b.Bills)
                .Include(o=>o.Orders)
                  .ThenInclude(u=>u.User)
                .Include(u=>u.Orders)
                    .ThenInclude(op=>op.OrderProducts)
                     .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(b => b.Id == id);
    

            if (table == null)
            {
                return NotFound();  // Eğer tablo bulunamazsa 404 sayfası döndür
            }

            return View(table);  // Bulunan tabloyu view'a gönder
        }

    }
}
