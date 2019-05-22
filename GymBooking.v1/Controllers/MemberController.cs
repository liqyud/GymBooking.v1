using System.Linq;
using System.Threading.Tasks;
using GymBooking.v1.Data;
using GymBooking.v1.Models;
using GymBooking.v1.ModelsIdentity;
using GymBooking.v1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymBooking.v1.Controllers
{
    [Authorize(Roles = "Member")]
    public class MemberController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly GymBookingContext _context;

        public MemberController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            GymBookingContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IActionResult> BookingList()
        {
            var applicationUser = await _userManager.GetUserAsync(HttpContext.User);
            if (applicationUser == null)
            {
                return NotFound();
            }

            applicationUser.GymClasses = _context.ApplicationUserGymClass.Where(a => a.ApplicationUserId == applicationUser.Id).ToList();
            foreach (var gymClassAttendingMember in applicationUser.GymClasses)
            {
                gymClassAttendingMember.GymClass = _context.GymClasses.FirstOrDefault(a => a.GymClassId == gymClassAttendingMember.GymClassId);
            }

            return View(applicationUser);
        }

        public enum BookMode
        {
            Book,
            Cancel,
            Show
        }

        public async Task<IActionResult> Book(int? id, string book, string cancel)
        {
            var model = new BookingViewModel();

            if (id == null)
            {
                return NotFound();
            }

            var bookMode = BookMode.Show;
            if (!string.IsNullOrEmpty(book))
            {
                bookMode = BookMode.Book;
            }
            else if (!string.IsNullOrEmpty(cancel))
            {
                bookMode = BookMode.Cancel;
            }

            var gymClass = await _context.GymClasses.Where(g => g.GymClassId == id).Include(a => a.AttendingMembers).FirstOrDefaultAsync();

            if (gymClass == null)
            {
                return NotFound();
            }

            if (!User.Identity.IsAuthenticated)
            {
                ViewBag.Message = "User Not Logged in";
                return RedirectToAction("Index", "Home");
            }

            var currentUser = _context.Users.FirstOrDefault(a => a.UserName == User.Identity.Name);
            if (currentUser == null)
            {
                ViewBag.Message = "User Not Found";
                return RedirectToAction("Index", "Home");
            }

            var booked = _context.ApplicationUserGymClass.FirstOrDefault(a => a.GymClassId == gymClass.GymClassId && a.ApplicationUserId == currentUser.Id);

            model.IsBooked = booked != null;

            switch (bookMode)
            {
                case BookMode.Book when booked != null:
                    model.Message = "Already Booked";
                    break;
                case BookMode.Book:
                    _context.ApplicationUserGymClass.Add(new ApplicationUserGymClass { GymClassId = gymClass.GymClassId, ApplicationUserId = currentUser.Id });
                    _context.SaveChanges();
                    model.Message = "Class has been Booked";
                    model.IsBooked = true;
                    break;
                case BookMode.Cancel when booked != null:
                    _context.ApplicationUserGymClass.Remove(booked);
                    _context.SaveChanges();
                    model.IsBooked = false;
                    model.Message = "Booking has been canceled";
                    break;
                case BookMode.Cancel:
                    model.Message = "Not Booked";
                    break;
            }

            model.Description = gymClass.Description;
            model.Duration = gymClass.Duration;
            model.Id = gymClass.GymClassId;
            model.Name = gymClass.Name;
            model.StartTime = gymClass.StartTime;
            return View(model);
        }
    }
}