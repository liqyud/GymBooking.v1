using GymBooking.v1.Data;
using GymBooking.v1.Models;
using GymBooking.v1.ModelsIdentity;
using GymBooking.v1.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GymBooking.v1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly GymBookingContext _context;

        public AdminController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            GymBookingContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IActionResult> MemberList()
        {
            return View(await _userManager.GetUsersInRoleAsync("Member"));
        }

        public IActionResult CreateMember()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMember(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                var member = new ApplicationUser
                {
                    Name = registerViewModel.Firstname + " " + registerViewModel.Lastname,
                    Email = registerViewModel.Email,
                    UserName = registerViewModel.Email,
                };

                var result = await _userManager.CreateAsync(member, registerViewModel.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(member, "Member");
                    ViewBag.message = "Member created successfully!";
                    return View();
                }

                AddErrors(result);
                ViewBag.message = "Error occured while trying to add the new member!";
                return View();
            }

            return View(registerViewModel);
        }

        public async Task<IActionResult> MemberDetails(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUsers.FirstOrDefaultAsync(g => g.Id == id);
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

        public async Task<IActionResult> EditMember(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUsers.FindAsync(id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            var user = new UserCredentialsViewModel
            {
                Id = applicationUser.Id,
                Name = applicationUser.Name,
                Email = applicationUser.Email,
                UserName = applicationUser.Email,
                Password = string.Empty
            };

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMember(string id, UserCredentialsViewModel viewModel)
        {
            var emailDoesExits = _context.ApplicationUsers.Any(x => x.Email == viewModel.Email && x.Id != viewModel.Id);

            if (emailDoesExits)
            {
                ViewBag.Message = $"User name {viewModel.Email} is already taken.";
            }

            if (ModelState.IsValid && !emailDoesExits)
            {
                var editedUserNewValue = await _context.ApplicationUsers.FindAsync(id);

                editedUserNewValue.Name = viewModel.Name;
                editedUserNewValue.Email = viewModel.Email;
                editedUserNewValue.UserName = viewModel.Email;

                try
                {

                    if (!string.IsNullOrWhiteSpace(viewModel.Password))
                    {
                        var user = await _userManager.FindByIdAsync(id);

                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                        await _userManager.ResetPasswordAsync(user, token, viewModel.Password);
                    }

                    await _userManager.UpdateAsync(editedUserNewValue);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ApplicationUsers.Any(g => g.Id == viewModel.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }

                return RedirectToAction("MemberList");
            }
            return View(viewModel);
        }

        public async Task<IActionResult> DeleteMember(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users.FirstOrDefaultAsync(g => g.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMember(string id, ApplicationUser applicationUser)
        {
            var user = await _context.ApplicationUsers.FindAsync(id);
            _context.ApplicationUsers.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("MemberList");
        }


        public IActionResult CreateGymClass()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGymClass([Bind("GymClassId,Name,Trainer,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gymClass);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(gymClass);
        }

        public async Task<IActionResult> GymClassDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses.FirstOrDefaultAsync(g => g.GymClassId == id);
            if (gymClass == null)
            {
                return NotFound();
            }
            gymClass.AttendingMembers = _context.ApplicationUserGymClass.Where(a => a.GymClassId == gymClass.GymClassId).ToList();
            foreach (var gymClassAttendingMember in gymClass.AttendingMembers)
            {
                gymClassAttendingMember.ApplicationUser = _context.ApplicationUsers.FirstOrDefault(a => a.Id == gymClassAttendingMember.ApplicationUserId);
            }
            return View(gymClass);
        }

        public async Task<IActionResult> EditGymClass(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses.FindAsync(id);

            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGymClass(int id, [Bind("GymClassId,Name,Trainer,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (id != gymClass.GymClassId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gymClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.GymClasses.Any(g => g.GymClassId == gymClass.GymClassId))
                    {
                        return NotFound();
                    }

                    throw;
                }
                return RedirectToAction("Index", "Home");
            }
            return View(gymClass);
        }


        public async Task<IActionResult> DeleteGymClass(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses.FirstOrDefaultAsync(g => g.GymClassId == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGymClass(int id)
        {
            var gymClass = await _context.GymClasses.FindAsync(id);
            _context.GymClasses.Remove(gymClass);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}