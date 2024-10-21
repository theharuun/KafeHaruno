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
            return View("Create"); // We return the "Create" view
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateBillViewModel viewModel)
        {
            // Areas to be checked
            var fieldsToValidate = new (string FieldName, object Value)[]
            {
                ("BillPrice", viewModel.BillPrice),
                ("IsPaid", viewModel.IsPaid),
                ("TableId", viewModel.TableId),
            };


            // Check every area
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


            // We take the relevant table
            var table = await context.Tables.FirstOrDefaultAsync(t => t.Id == viewModel.TableId);
            if (table == null)
            {
                return NotFound("Table not found.");
            }

            // We save the Bill
            await context.Bills.AddAsync(bill);
            await context.SaveChangesAsync();

            // We mark that the table is full 
            table.IsFull = true;
            await context.SaveChangesAsync(); // We save the changes made

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


            // Find user in Database
            var bill = await context.Bills.FindAsync(viewModel.Id);
            if (bill != null)
            {
                // Update users' information
                bill.BillPrice = viewModel.BillPrice;
                bill.IsPaid = viewModel.IsPaid;
                bill.TableId = viewModel.TableId;

                // Let's take the relevant table
                var table = await context.Tables.FirstOrDefaultAsync(t => t.Id == viewModel.TableId);
                if (table == null)
                {
                    return NotFound("Table not found.");
                }
                // Save changes
                await context.SaveChangesAsync();
                bill.Tables.IsFull = !viewModel.IsPaid;
                await context.SaveChangesAsync();

                // Set success message
                TempData["SuccessMessage"] = "Bill updated successfully.";

            }
            else
            {
                // Set error message if user not found
                TempData["ErrorMessage"] = "Bill not found.";
            }

            // Redirect to admin page
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
                return NotFound(); //Return 404 if user not found
            }

            return View(bill); // Rotate view to view details
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

            // When the Bill is paid, find and delete the Order records from the Table attached to it
            var tableId = bill.Tables.Id;
            var orders = context.Orders.Where(o => o.TableId == tableId).ToList();
            context.Orders.RemoveRange(orders); // Delete all found Order records

            bill.IsPaid = true;
            bill.Tables.IsFull = false;

            await context.SaveChangesAsync(); // Save changes

            return RedirectToAction("AdminIndex", "Table");
        }



    }
}
