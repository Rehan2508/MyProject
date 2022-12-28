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
    public class SalesController : Controller
    {
        private readonly CateringContext _context;
        const string checkTotalCost = "_Cost";

        public SalesController(CateringContext context)
        {
            _context = context;
        }

        // GET: Sales
        public async Task<IActionResult> Index()
        {
              return View();
        }

        [Authorize(Roles ="admin")]
        public async Task<IActionResult> SaleAmountInCurrentMonth()
        {
            return View();
        }

        [HttpPost,ActionName("SaleAmountInCurrentMonth")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SaleAmount()
        {
            var sale = await _context.sales.ToListAsync();

            double totalAmount = 0;
            if (sale != null)
            {
                string[] strArr = Request.Form["saleMonth"].ToString().Split('-');
                foreach (var item in sale) 
                { 
                    if(item.date.Month.ToString() == strArr[1])
                        totalAmount += item.totalPrice;
                }
            }
            HttpContext.Session.SetString(checkTotalCost, totalAmount.ToString());
            return View();
        }

        [Authorize(Roles ="admin")]
        public async Task<IActionResult> CustomerListInCurrentMonth()
        {
            List<Sale> lsale = new List<Sale>();
            return View(lsale);
        }

        [HttpPost, ActionName("CustomerListInCurrentMonth")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CustomerList()
        {
            string[] strArr = Request.Form["saleMonth"].ToString().Split('-');
            var clist = await _context.sales.Where(e => e.date.Month.ToString() == strArr[1]).ToListAsync();
            var customers = clist.DistinctBy(c => c.userId);
            return View(customers);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CountOfSalesInCurrentMonth()
        {
            List<ProductSold> lsale = new List<ProductSold>();
            return View(lsale);
        }

        [Authorize(Roles = "admin")]
        [HttpPost,ActionName("CountOfSalesInCurrentMonth")]
        public async Task<IActionResult> CountOfSales()
        {
            string[] strArr = Request.Form["saleMonth"].ToString().Split('-');
            var productSold = await _context.productsSold.Where(e => e.sale.date.Month.ToString() == strArr[1]).GroupBy(e => e.pid).Select(x => new
            {
                x.Key,
                Sum = x.Sum( s => s.quantity)
            }).ToListAsync();

            List<ProductSold> products = new List<ProductSold>();
            foreach (var product in productSold)
            {
                Product prod = await _context.products.FindAsync(product.Key);
                ProductSold p = new ProductSold() 
                {
                    pid = product.Key,
                    product = prod,
                    quantity = product.Sum
                };
                products.Add(p);
            }
            return View(products);
        }

        // GET: Sales/Details/5
        /*public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.sales == null)
            {
                return NotFound();
            }

            var sale = await _context.sales
                .FirstOrDefaultAsync(m => m.id == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }*/

        // GET: Sales/Create
        /*public IActionResult Create()
        {
            return View();
        }

        // POST: Sales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,date,totalPrice")] Sale sale)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sale);
        }
*/
        // GET: Sales/Edit/5
        /*public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.sales == null)
            {
                return NotFound();
            }

            var sale = await _context.sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }
            return View(sale);
        }

        // POST: Sales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,date,totalPrice")] Sale sale)
        {
            if (id != sale.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sale);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleExists(sale.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sale);
        }
*/
        // GET: Sales/Delete/5
        /*public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.sales == null)
            {
                return NotFound();
            }

            var sale = await _context.sales
                .FirstOrDefaultAsync(m => m.id == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.sales == null)
            {
                return Problem("Entity set 'CateringContext.sales'  is null.");
            }
            var sale = await _context.sales.FindAsync(id);
            if (sale != null)
            {
                _context.sales.Remove(sale);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/

        private bool SaleExists(int id)
        {
          return _context.sales.Any(e => e.id == id);
        }
    }
}
