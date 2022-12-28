using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CateringWebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace CateringWebApplication.Controllers
{
    public class ProductsController : Controller
    {
        private readonly CateringContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public ProductsController(CateringContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        // GET: Products
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {
            var cateringContext = _context.products.Include(p => p.category);
            return View(await cateringContext.ToListAsync());
        }

        [Authorize(Roles = "customer")]
        public async Task<IActionResult> SelectedProducts(int? id)
        {
            var cateringContext = _context.products.Include(p => p.category).Where(e => e.categoryId == id);
            return View(await cateringContext.ToListAsync());
        }

        [HttpPost]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> SelectedProducts(List<Product> plist)
        {
            try
            {
                foreach (Product p in plist)
                {
                    if (p.check)
                    {
                        Cart c = new Cart();
                        c.pid = p.id;
                        c.totalAmount = p.price - (p.price * (p.discount / 100));
                        c.quantity = 1;
                        c.userId = _contextAccessor.HttpContext.User.Identity.Name;
                        _context.carts.Add(c);
                        //p1.name = p.name;
                        //p1.price = p.price;
                        //p1.qty = 10;
                        //p1.prodID = p.id;
                        //db.purchases.Add(p1);
                    }
                }
                _context.SaveChanges();
                //Session["Items"] = db.purchases.ToList().Count();
                TempData["Message"] = "Success";
                return RedirectToAction("SelectCategory", "Categories");
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                return View(plist);
            }
        }

        // GET: Products/Details/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.products == null)
            {
                return NotFound();
            }

            var product = await _context.products
                .Include(p => p.category)
                .FirstOrDefaultAsync(m => m.id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["categoryId"] = new SelectList(_context.categories, "id", "name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);

            FileInfo fileInfo = new FileInfo(product.formFile.FileName);
            //string fileName = product.formFile.FileName + fileInfo.Extension;
            string fileNameWithPath = Path.Combine(path, product.formFile.FileName);
            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                product.formFile.CopyTo(stream);
            }
                        
            Product p = new Product()
            {
                id = product.id,
                name = product.name,
                description = product.description,
                imagePath = "~/Images/" + product.formFile.FileName,
                price = product.price,
                discount = product.discount,
                quantity = product.quantity,
                categoryId = product.categoryId,
            };

            
            
            //if (ModelState.IsValid)
            //{
                _context.Add(p);
                await _context.SaveChangesAsync();

            
            Inventory inventory = new Inventory()
            {
                quantity = p.quantity,
                pid = p.id,
                cid = p.categoryId
            };

            _context.inventories.Add(inventory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            //}
            //ViewData["categoryId"] = new SelectList(_context.categories, "id", "id", product.categoryId);
            //return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.products == null)
            {
                return NotFound();
            }

            var product = await _context.products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["categoryId"] = new SelectList(_context.categories, "id", "name", product.categoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.id)
            {
                return NotFound();
            }

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            FileInfo fileInfo = new FileInfo(product.formFile.FileName);
            //string fileName = product.formFile.FileName + fileInfo.Extension;
            string fileNameWithPath = Path.Combine(path, product.formFile.FileName);
            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                product.formFile.CopyTo(stream);
            }

            Product p = new Product()
            {
                id = product.id,
                name = product.name,
                description = product.description,
                imagePath = "~/Images/" + product.formFile.FileName,
                price = product.price,
                discount = product.discount,
                //quantity = 10,
                categoryId = product.categoryId,
            };

            //if (ModelState.IsValid)
            //{
                try
                {
                    _context.Update(p);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            //}
            //ViewData["categoryId"] = new SelectList(_context.categories, "id", "id", product.categoryId);
            //return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.products == null)
            {
                return NotFound();
            }

            var product = await _context.products
                .Include(p => p.category)
                .FirstOrDefaultAsync(m => m.id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.products == null)
            {
                return Problem("Entity set 'CateringContext.products'  is null.");
            }
            var product = await _context.products.FindAsync(id);
            var inventory = await _context.inventories.Where(e => e.pid == id).ToListAsync();
            if (product != null)
            {
                _context.products.Remove(product);
                
            }
            if(inventory != null)
            {
                _context.inventories.RemoveRange(inventory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return _context.products.Any(e => e.id == id);
        }
    }
}
