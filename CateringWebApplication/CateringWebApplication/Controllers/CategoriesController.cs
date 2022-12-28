using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CateringWebApplication.Models;
using Microsoft.AspNetCore.Authorization;

namespace CateringWebApplication.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly CateringContext _context;

        public CategoriesController(CateringContext context)
        {
            _context = context;
        }

        // GET: Categories
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {
              return View(await _context.categories.ToListAsync());
        }

        [Authorize(Roles = "customer")]
        public async Task<IActionResult> SelectCategory()
        {
            return View(await _context.categories.ToListAsync());
        }

        // GET: Categories/Details/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.categories == null)
            {
                return NotFound();
            }

            var category = await _context.categories
                .FirstOrDefaultAsync(m => m.id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");
            //create folder if not exist
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //get file extention
            FileInfo fileInfo = new FileInfo(category.formFile.FileName);
            string fileName = category.formFile.FileName + fileInfo.Extension;
            string fileNameWithPath = Path.Combine(path, category.formFile.FileName);
            using(var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                category.formFile.CopyTo(stream);
            }

            Category c = new Category()
            {
                id = category.id,
                name = category.name,   
                description = category.description,
                imagePath = "~/Images/" + category.formFile.FileName
            };

            //if (ModelState.IsValid)
            //{
                _context.Add(c);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            //}
            //return View(category);
        }

        // GET: Categories/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.categories == null)
            {
                return NotFound();
            }

            var category = await _context.categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.id)
            {
                return NotFound();
            }

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");
            //create folder if not exist
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //get file extention
            FileInfo fileInfo = new FileInfo(category.formFile.FileName);
            string fileName = category.formFile.FileName + fileInfo.Extension;
            string fileNameWithPath = Path.Combine(path, category.formFile.FileName);
            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                category.formFile.CopyTo(stream);
            }

            Category c = new Category()
            {
                id = category.id,
                name = category.name,
                description = category.description,
                imagePath = "~/Images/" + category.formFile.FileName
            };

            //if (ModelState.IsValid)
            //{
                try
                {
                    _context.Update(c);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.id))
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
            //return View(category);
        }

        // GET: Categories/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.categories == null)
            {
                return NotFound();
            }

            var category = await _context.categories
                .FirstOrDefaultAsync(m => m.id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.categories == null)
            {
                return Problem("Entity set 'CateringContext.categories'  is null.");
            }
            var category = await _context.categories.FindAsync(id);
            if (category != null)
            {
                _context.categories.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return _context.categories.Any(e => e.id == id);
        }
    }
}
