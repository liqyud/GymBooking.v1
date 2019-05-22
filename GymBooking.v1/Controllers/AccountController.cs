using GymBooking.v1.Data;
using GymBooking.v1.ModelsIdentity;
using GymBooking.v1.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymBooking.v1.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly GymBookingContext _context;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            GymBookingContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) return View(loginViewModel);
            var result = await _signInManager.PasswordSignInAsync(loginViewModel.Username, loginViewModel.Password, false, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt");

            return View(loginViewModel);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}