using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminPanelTutorial.Data;
using AdminPanelTutorial.Models;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Rendering;
using AdminPanelTutorial.Models.ViewModels;

namespace AdminPanelTutorial.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment  )
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products (Index)
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        // GET: Products/Show/5
        public async Task<IActionResult> Show(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            productVM productVm = new productVM()
            {
                Product = product
            };

            return View(productVm);
        }


        // GET: Products/Create
        public IActionResult Upsert(int?id)
        {
            productVM Productvm = new()
            {
                CategoryList = _context.Categories.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value=u.Id.ToString()
                }),
                Product= new Product()
            };
            if (id == null || id == 0)
            {
                return View(Productvm);
            }
            else
            {
                Productvm.Product = _context.Products.FirstOrDefault(u => u.Id == id);
                return View(Productvm);
            }
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
      
        public async Task<IActionResult> Upsert(productVM productVm, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    // Generate a unique file name
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, "images", "product");

                    // Ensure the directory exists
                    if (!Directory.Exists(productPath))
                    {
                        Directory.CreateDirectory(productPath);
                    }

                    // Delete old image if updating an existing product
                    if (!string.IsNullOrEmpty(productVm.Product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, productVm.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Save new image
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    productVm.Product.ImageUrl = @"\images\product\" + fileName;
                }

                // Check if it's an update or a new insert
                if (productVm.Product.Id == 0)
                {
                    productVm.Product.CreatedAt = DateTime.Now;
                    productVm.Product.UpdatedAt = DateTime.Now;
                    _context.Products.Add(productVm.Product);
                }
                else
                {
                    var existingProduct = await _context.Products.FindAsync(productVm.Product.Id);
                    if (existingProduct != null)
                    {
                        existingProduct.Name = productVm.Product.Name;
                        existingProduct.Description = productVm.Product.Description;
                        existingProduct.SKU = productVm.Product.SKU;
                        existingProduct.Barcode = productVm.Product.Barcode;
                        existingProduct.Price = productVm.Product.Price;
                        existingProduct.CostPrice = productVm.Product.CostPrice;
                        existingProduct.QuantityInStock = productVm.Product.QuantityInStock;
                        existingProduct.CategoryId = productVm.Product.CategoryId;
                        existingProduct.ImageUrl = productVm.Product.ImageUrl ?? existingProduct.ImageUrl;
                        existingProduct.UpdatedAt = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["success"] = "Product saved successfully!";
                return RedirectToAction(nameof(Index));
            }

            // Repopulate CategoryList if validation fails
            productVm.CategoryList = _context.Categories.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }).ToList();

            return View(productVm);
        }

        //// GET: Products/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Products.FindAsync(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
        //    return View(product);
        //}

        //// POST: Products/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,SKU,Barcode,Price,CostPrice,QuantityInStock,CategoryId,CreatedAt,UpdatedAt")] Product product)
        //{
        //    if (id != product.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            product.UpdatedAt = DateTime.Now;
        //            _context.Update(product);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ProductExists(product.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }

        //        return RedirectToAction(nameof(Index));
        //    }

        //    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
        //    return View(product);
        //}

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
