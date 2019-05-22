using GymBooking.v1.Data;
using GymBooking.v1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GymBooking.v1.Controllers
{
    public class HomeController : Controller
    {
        private readonly GymBookingContext _context;

        public HomeController(GymBookingContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.GymClasses.OrderBy(gymClass => gymClass.StartTime).ToListAsync());
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
