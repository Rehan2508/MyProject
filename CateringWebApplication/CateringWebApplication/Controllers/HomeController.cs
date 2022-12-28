using CateringWebApplication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace CateringWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly CateringContext _context;

        private readonly IHttpContextAccessor _contextAccessor;
        const string checkRole = "_Role";
        const string cartCount = "_cartCount";
        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor contextAccessor,CateringContext context)
        {
            _logger = logger;
            _context = context;
            //_logger.Log()
            _contextAccessor = contextAccessor;
        }

        public IActionResult Index()
        {
            int count = _context.carts.ToList().Count();
            HttpContext.Session.SetInt32(cartCount, count);
            /*var user = _contextAccessor.HttpContext.User;
            string userid = user.Identity.Name;
            if ()
            {

            }*/
            var role = _contextAccessor.HttpContext;
            string check = role.User.IsInRole("admin") ? "admin" : "customer";
            HttpContext.Session.SetString(checkRole, check);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}