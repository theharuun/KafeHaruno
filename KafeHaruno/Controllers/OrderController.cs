using KafeHaruno.Entities;
using KafeHaruno.KafeHarunoDbContext;
using KafeHaruno.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KafeHaruno.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext context;

        public OrderController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AdminIndex()
        {
            var list = context.Orders
           .Include(t => t.Tables)
           .Include(u => u.User)
           .Include(op => op.OrderProducts)
               .ThenInclude(p => p.Product)
           .OrderByDescending(o => o.Id) 
           .ToList();


            return View(list);
        }

        [HttpGet]
        public IActionResult Create(int tableId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Veritabanındaki ürünleri getiriyoruz (bunlar kullanıcıya gösterilecek)
            var products = context.Products.Select(p => new ProductViewModel
            {
                ProductId = p.Id,
                ProductName=p.ProductName,
                ProductPrice = p.Price,
                Quantity = 0 // Başlangıçta miktar sıfır
            }).ToList();

            var viewModel = new OrderViewModel
            {
                OrderPayment=0,
                TableId = tableId,
                UserId = userId,
                Products = products
            };

            return View(viewModel);
        }


        /// burada order kaydedilirken bir hata alıyoruz table.orders.orderproduct boş gözüküyor
        public async Task<IActionResult> Create(OrderViewModel viewModel)
        {
            
           // Formdan gelen Products listesini kontrol edelim
            if (viewModel.Products == null || !viewModel.Products.Any())
            {
                ModelState.AddModelError(string.Empty, "Ürün listesi boş veya geçersiz.");
                return View(viewModel);
            }

            var table = await context.Tables.FirstOrDefaultAsync(t => t.Id == viewModel.TableId);
            if (table == null)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz masa ID'si.");
                return View(viewModel);
            }


            // Order nesnesini oluşturuyoruz
            var order = new Order()
            {
                UserId = viewModel.UserId,
                TableId = viewModel.TableId,
                OrderPayment = 1,
                Tables=table,
                OrderProducts = new List<OrderProduct>() { }// Boş liste ile başlıyoruz
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync();
            // Siparişi masaya ekleyin
            table.Orders.Add(order);
            await context.SaveChangesAsync();


            // Sadece Quantity > 0 olan ürünleri seçiyoruz
            var orderProducts = viewModel.Products
                .Where(p => p.Quantity > 0) // Sadece seçilen ürünler
                .Select(p => new OrderProduct
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity,
                    OrderId=order.Id
                }).ToList();

            // Eğer hiç ürün seçilmemişse hata döndürüyoruz
            if (!orderProducts.Any())
            {
                ModelState.AddModelError(string.Empty, "Lütfen en az bir ürün seçin.");
                return View(viewModel);
            }

   // Total price hesaplaması
            int totalPrice = (int)(orderProducts.Sum(op =>
                    op.Quantity * context.Products.First(p => p.Id == op.ProductId).Price) ?? 0);

            order.OrderPayment = totalPrice; // OrderPayment'ı güncelle
            order.Tables.IsFull = true; // Masanın dolu olduğunu işaretle

            foreach (var orderP in orderProducts)
            {
                Console.WriteLine(orderP.ProductId);

                orderP.Order = order;
                order.OrderProducts.Add(orderP);
                await context.SaveChangesAsync();

            }



         
   

            // Mevcut fatura kontrolü
            var bill = await context.Bills.FirstOrDefaultAsync(b => b.TableId == order.TableId && !b.IsPaid);

            if (bill != null)
            {
                // Eğer mevcut bir fatura varsa, fiyatı güncelle
                bill.BillPrice += totalPrice; // Yeni siparişin toplam fiyatını ekleyin
                await context.SaveChangesAsync(); // Güncellemeyi kaydet
            }
            else
            {
                // Yeni bir fatura oluşturmak için CreateBillViewModel kullanarak yönlendirme yapıyoruz
                var billViewModel = new CreateBillViewModel
                {
                    BillPrice = totalPrice,
                    IsPaid = false,
                    TableId = order.TableId,
                };

                return RedirectToAction("CreateBillForm", "Bill", billViewModel); // Yeni fatura oluşturma
            }

          



            // Eğer bir fatura güncellenmişse, TableController'a geri dön
            return RedirectToAction("UserIndex", "Table");

        }






    }
}
