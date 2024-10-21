using KafeHaruno.Entities;
using KafeHaruno.KafeHarunoDbContext;
using KafeHaruno.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KafeHaruno.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext context;

        public ProductController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AdminIndex()
        {
            var list=context.Products
                .Include(pt=>pt.ProductType)
               .OrderByDescending(o => o.Id)
               .ToList();
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            var productTypes = await context.ProductTypes.ToListAsync(); // Get product types from database
            ViewBag.ProductTypes = productTypes; // Assign product types to ViewBag
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductViewModel viewModel)
        {
            // Areas to be checked
            var fieldsToValidate = new (string FieldName, object Value)[]
            {
                ("ProductName", viewModel.ProductName),
                ("ProductDescription", viewModel.ProductDescription),
                ("Price", viewModel.Price),
                ("ProductTypeId", viewModel.ProductTypeId)
            };


            // Check each field
            foreach (var field in fieldsToValidate)
            {
                if (field.Value == null)
                {
                    ModelState.AddModelError(field.FieldName, $"{field.FieldName} is required!");
                }
            }


            var product = new Product
            {
                ProductName = viewModel.ProductName,
                ProductDescription = viewModel.ProductDescription,
                ProductTypeId = viewModel.ProductTypeId,
                Price = viewModel.Price,
            };

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            return RedirectToAction("AdminIndex", "Product");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await context.Products.Include(p => p.ProductType)
                                                 .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new AddProductViewModel
            {
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                Price = product.Price,
                ProductTypeId = product.ProductTypeId
            };

            // Get product types
            var productTypes = await context.ProductTypes.ToListAsync();
            ViewBag.ProductTypes = productTypes;

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddProductViewModel viewModel)
        {
    
            var product = await context.Products.FindAsync(viewModel.Id);

            if (product == null)
            {
                return NotFound();
            }

            product.ProductName = viewModel.ProductName;
            product.ProductDescription = viewModel.ProductDescription;
            product.Price = viewModel.Price;
            product.ProductTypeId = viewModel.ProductTypeId;

            await context.SaveChangesAsync();

            return RedirectToAction("AdminIndex", "Product");
        }


        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await context.Products.FindAsync(id);

            if (product == null)
            {
                return RedirectToAction("AdminIndex", "Product");
            }
            context.Products.Remove(product);
            await context.SaveChangesAsync();



            return RedirectToAction("AdminIndex", "Product");

        }

    }
}
