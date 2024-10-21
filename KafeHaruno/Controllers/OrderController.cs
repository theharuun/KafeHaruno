using Humanizer;
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

            // We fetch products from the database (they will be displayed to the user)
            var products = context.Products.Select(p => new ProductViewModel
            {
                ProductId = p.Id,
                ProductName=p.ProductName,
                ProductPrice = p.Price,
                Quantity = 0 // Initially the amount is zero
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


        /// Here we get an error while saving the order, table.orders.orderproduct appears empty
        public async Task<IActionResult> Create(OrderViewModel viewModel)
        {

            // Let's check the Products list from the form
            if (viewModel.Products == null || !viewModel.Products.Any())
            {
                ModelState.AddModelError(string.Empty, "The product list is empty or invalid.");
                return View(viewModel);
            }

            var table = await context.Tables.FirstOrDefaultAsync(t => t.Id == viewModel.TableId);
            if (table == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid table ID.");
                return View(viewModel);
            }


            // We create the Order object
            var order = new Order()
            {
                UserId = viewModel.UserId,
                TableId = viewModel.TableId,
                OrderPayment = 1,
                Tables=table,
                OrderProducts = new List<OrderProduct>() { }// We start with an empty list
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync();
            // Add order to table
            table.Orders.Add(order);
            await context.SaveChangesAsync();


            // We only select products with Quantity > 0
            var orderProducts = viewModel.Products
                .Where(p => p.Quantity > 0) // Selected products only
                .Select(p => new OrderProduct
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity,
                    OrderId=order.Id
                }).ToList();

            // If no product is selected we return an error
            if (!orderProducts.Any())
            {
                ModelState.AddModelError(string.Empty, "Please select at least one product.");
                return View(viewModel);
            }

           // Calculating of total price
            int totalPrice = (int)(orderProducts.Sum(op =>
                    op.Quantity * context.Products.First(p => p.Id == op.ProductId).Price) ?? 0);

            order.OrderPayment = totalPrice; // Update OrderPayment
            order.Tables.IsFull = true; // Mark the table full

            foreach (var orderP in orderProducts)
            {
                Console.WriteLine(orderP.ProductId);

                orderP.Order = order;
                order.OrderProducts.Add(orderP);
                await context.SaveChangesAsync();

            }






            // Check current invoice
            var bill = await context.Bills.FirstOrDefaultAsync(b => b.TableId == order.TableId && !b.IsPaid);

            if (bill != null)
            {
                // If there is an existing invoice, update the price
                bill.BillPrice += totalPrice; // Add the total price of the new order
                await context.SaveChangesAsync(); // Save Changes
            }
            else
            {
                // To create a new invoice we use CreateBillViewModel to redirect
                var billViewModel = new CreateBillViewModel
                {
                    BillPrice = totalPrice,
                    IsPaid = false,
                    TableId = order.TableId,
                };

                return RedirectToAction("CreateBillForm", "Bill", billViewModel); // Yeni fatura oluşturma
            }





            // If an invoice is updated, return to TableController
            return RedirectToAction("UserIndex", "Table");

        }






    }
}
