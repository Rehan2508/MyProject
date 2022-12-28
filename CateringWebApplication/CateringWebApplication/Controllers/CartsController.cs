using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CateringWebApplication.Models;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using MimeKit;
using MailKit.Net.Smtp;

namespace CateringWebApplication.Controllers
{
    public class CartsController : Controller
    {
        private readonly CateringContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public CartsController(CateringContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        // GET: Carts
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> Index()
        {
            /*if (_contextAccessor.HttpContext.User.Identity.Name != null)
            {*/
                var user = _contextAccessor.HttpContext.User.Identity.Name;

                var cart = await _context.carts.Where(e => e.userId == user).ToListAsync();
                double totalCost = 0;
                if (cart != null)
                {
                    foreach (var product in cart)
                    {
                        totalCost += product.totalAmount;
                    }
                }
                ViewBag.TotalCost = totalCost;

                var cateringContext = _context.carts.Include(c => c.product).Where(e => e.userId == user);

                return View(await cateringContext.ToListAsync());
            //}
        }

        public JsonResult GetUsers()
        {
            var userList = _context.carts.Select(c => c.userId).ToList();
            return Json(userList);
        }

        [Authorize(Roles = "customer")]
        public async Task<IActionResult> Add(int? id)
        {
            if (id == null || _context.carts == null)
            {
                return NotFound();
            }

            var cart = await _context.carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            var inventory = await _context.inventories.Where(e => e.pid == cart.pid).ToListAsync();
            if(inventory != null)
            {
                var i = inventory[0];
                if(i.quantity > cart.quantity) 
                {
                    cart.quantity += 1;

                    var product = await _context.products.FindAsync(cart.pid);
                    cart.totalAmount += (product.price - (product.price * product.discount / 100));

                    _context.carts.Update(cart);
                    await _context.SaveChangesAsync();
                }
            }
            

            
            //ViewData["pid"] = new SelectList(_context.products, "id", "id", cart.pid);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "customer")]
        public async Task<IActionResult> Decrease(int? id)
        {
            if (id == null || _context.carts == null)
            {
                return NotFound();
            }

            var cart = await _context.carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            if (cart.quantity > 1)
            {
                cart.quantity -= 1;
                var product = await _context.products.FindAsync(cart.pid);
                cart.totalAmount -= (product.price - (product.price * product.discount / 100));
                _context.carts.Update(cart);
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
            //ViewData["pid"] = new SelectList(_context.products, "id", "id", cart.pid);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "customer")]
        public async Task<IActionResult> CheckOut()
        {
            var cart = await _context.carts.Where(e => e.userId == _contextAccessor.HttpContext.User.Identity.Name).ToListAsync();

            if (cart == null)
            {
                return NotFound();
            }

            
            double totalCost = 0;
            if (cart != null)
            {
                //calculate sale
                foreach (var product in cart)
                {
                    totalCost += product.totalAmount;

                    //update Inventory
                    var inventory = await _context.inventories.Where(e => e.pid == product.pid).ToListAsync();
                    //_context.inventories.UpdateRange(inventory);
                    foreach (var item in inventory)
                    {
                        item.quantity -= product.quantity;
                        _context.inventories.Update(item);
                        //give notification to admin about low product quantity
                        if (item.quantity < 5)
                        {
                            //message the admin
                            var message = new MimeMessage();
                            message.From.Add(new MailboxAddress("Project Admin", "testproject456789@gmail.com"));
                            message.To.Add(new MailboxAddress("Project Admin", "rehandilkash619@gmail.com"));
                            message.Subject = "Reorder  ";
                            message.Body = new TextPart("plain")
                            {
                                Text = "Please reorder : "// + item.product.name
                            };
                            using(var client = new SmtpClient())
                            {
                                client.Connect("smtp.gmail.com", 587, false);
                                await client.AuthenticateAsync("testproject456789@gmail.com", "hjpvtclmpaylevzb").ConfigureAwait(false); ;
                                await client.SendAsync(message).ConfigureAwait(false);
                                client.Disconnect(true);
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                    
                }

                //update Sale
                Sale sale = new Sale() 
                { 
                    date = DateTime.Now,
                    totalPrice = totalCost,
                    userId = _contextAccessor.HttpContext.User.Identity.Name
                };

                _context.sales.AddAsync(sale); 
                await _context.SaveChangesAsync();

                //update ProductSold
                foreach(var item in cart)
                {
                    ProductSold ps = new ProductSold()
                    {
                        sid = sale.id,
                        pid = item.pid,
                        quantity = item.quantity
                    };
                    _context.productsSold.AddAsync(ps);
                }
                await _context.SaveChangesAsync();
                

                //empty the cart
                _context.carts.RemoveRange(cart);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public JsonResult CalculateTotal()
        {
            var cart = _context.carts.Where(e => e.userId == _contextAccessor.HttpContext.User.Identity.Name).ToList();
            double totalCost = 0;
            if (cart != null)
            {

                foreach (var item in cart)
                {
                    totalCost += item.totalAmount;
                }
            }

            return Json(totalCost);
        }

        [Authorize(Roles = "customer")]
        public async Task<IActionResult> EmptyCart()
        {
            var cart = _context.carts.Where(e => e.userId == _contextAccessor.HttpContext.User.Identity.Name);
            _context.carts.RemoveRange(cart);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Carts/Delete/5
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.carts == null)
            {
                return NotFound();
            }

            var cart = await _context.carts
                .Include(c => c.product).Where(e => e.userId == _contextAccessor.HttpContext.User.Identity.Name)
                .FirstOrDefaultAsync(m => m.id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "customer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.carts == null)
            {
                return Problem("Entity set 'CateringContext.carts'  is null.");
            }
            var cart = await _context.carts.FindAsync(id);
            if (cart != null)
            {
                _context.carts.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.carts.Any(e => e.id == id);
        }

        // GET: Carts/Details/5
        /*public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.carts == null)
            {
                return NotFound();
            }

            var cart = await _context.carts
                .Include(c => c.product)
                .FirstOrDefaultAsync(m => m.id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }*/

        // GET: Carts/Create
        /*public IActionResult Create()
        {
            ViewData["pid"] = new SelectList(_context.products, "id", "id");
            return View();
        }*/

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,quantity,pid,totalAmount")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["pid"] = new SelectList(_context.products, "id", "id", cart.pid);
            return View(cart);
        }*/

        // GET: Carts/Edit/5
        /*public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.carts == null)
            {
                return NotFound();
            }

            var cart = await _context.carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["pid"] = new SelectList(_context.products, "id", "id", cart.pid);
            return View(cart);
        }*/



        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,quantity,pid,totalAmount")] Cart cart)
        {
            if (id != cart.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.id))
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
            ViewData["pid"] = new SelectList(_context.products, "id", "id", cart.pid);
            return View(cart);
        }
*/

    }
}
