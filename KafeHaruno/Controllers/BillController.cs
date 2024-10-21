using KafeHaruno.Entities;
using KafeHaruno.KafeHarunoDbContext;
using KafeHaruno.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace KafeHaruno.Controllers
{
    public class BillController : Controller
    {
        private readonly ApplicationDbContext context;

        public BillController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AdminIndex()
        {
            return View(context.Bills.ToList());
        }

        [HttpGet]
        public IActionResult CreateBillForm(CreateBillViewModel viewModel)
        {
            return View("Create"); // "Create" view'ini döndürüyoruz
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateBillViewModel viewModel)
        {
            // Kontrol edilecek alanlar
            var fieldsToValidate = new (string FieldName, object Value)[]
            {
                ("BillPrice", viewModel.BillPrice),
                ("IsPaid", viewModel.IsPaid),
                ("TableId", viewModel.TableId),
            };


            // Her alanı kontrol et
            foreach (var field in fieldsToValidate)
            {
                if (field.Value == null)
                {
                    ModelState.AddModelError(field.FieldName, $"{field.FieldName} is required!");
                }
            }




            var bill = new Bill
            {

                BillPrice = viewModel.BillPrice,
                IsPaid = viewModel.IsPaid,
                TableId = viewModel.TableId,
               
            };


            // İlgili masayı alıyoruz
            var table = await context.Tables.FirstOrDefaultAsync(t => t.Id == viewModel.TableId);
            if (table == null)
            {
                return NotFound("Table not found.");
            }

            // Faturayı kaydediyoruz
            await context.Bills.AddAsync(bill);
            await context.SaveChangesAsync();

            // Masanın dolu olduğunu işaretliyoruz
            table.IsFull = true;
            await context.SaveChangesAsync(); // Yapılan değişiklikleri kaydediyoruz

            return RedirectToAction("UserIndex", "Table");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var bill = await context.Bills.FindAsync(id);

            return View(bill);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Bill viewModel)
        {


            // Kullanıcıyı veritabanında bul
            var bill = await context.Bills.FindAsync(viewModel.Id);
            if (bill != null)
            {
                // Kullanıcı bilgilerini güncelle
                bill.BillPrice = viewModel.BillPrice;
                bill.IsPaid = viewModel.IsPaid;
                bill.TableId = viewModel.TableId;

                // İlgili masayı alıyoruz
                var table = await context.Tables.FirstOrDefaultAsync(t => t.Id == viewModel.TableId);
                if (table == null)
                {
                    return NotFound("Table not found.");
                }
                // Değişiklikleri kaydet
                await context.SaveChangesAsync();
                bill.Tables.IsFull = !viewModel.IsPaid;
                await context.SaveChangesAsync();
                // Başarı mesajını ayarla
                TempData["SuccessMessage"] = "Bill updated successfully.";

            }
            else
            {
                // Kullanıcı bulunamadıysa hata mesajı ayarla
                TempData["ErrorMessage"] = "Bill not found.";
            }

            // Admin sayfasına yönlendir
            return RedirectToAction("AdminIndex", "Bill");
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
         
            var bill = await context.Bills
                 .Include(b => b.Tables)
                     .ThenInclude(t => t.Orders) // Include Orders related to Tables
                         .ThenInclude(o => o.OrderProducts) // Include OrderProducts related to Orders
                             .ThenInclude(op => op.Product) // Include Products related to OrderProducts
                 .FirstOrDefaultAsync(b => b.Id == id);


            if (bill == null)
            {
                return NotFound(); // Kullanıcı bulunamazsa 404 döndür
            }

            return View(bill); // Detayları görüntülemek için view'ı döndür
        }


        public async Task<IActionResult> Delete(int id)
        {
            var bill = await context.Bills.FindAsync(id);

            if (bill == null)
            {
                return RedirectToAction("AdminIndex", "Bill");
            }
            context.Bills.Remove(bill);
            await context.SaveChangesAsync();

    

            return RedirectToAction("AdminIndex", "Bill");

        }

        public async Task<IActionResult> PayToBill(int id)
        {
            var bill = await context.Bills.Include(t => t.Tables).FirstOrDefaultAsync(a => a.Id == id);
            if (bill == null)
            {
                return RedirectToAction("AdminIndex", "Table");
            }

            // Bill ödendiğinde, ona bağlı Table üzerinden Order kayıtlarını bul ve sil
            var tableId = bill.Tables.Id;
            var orders = context.Orders.Where(o => o.TableId == tableId).ToList();
            context.Orders.RemoveRange(orders); // Bulunan tüm Order kayıtlarını sil

            bill.IsPaid = true;
            bill.Tables.IsFull = false;

            await context.SaveChangesAsync(); // Değişiklikleri kaydet

            return RedirectToAction("AdminIndex", "Table");
        }



    }
}
