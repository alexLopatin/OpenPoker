using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenPoker.Infrastructure;
using OpenPoker.Models;

namespace OpenPoker.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationContext db;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationContext applicationContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            db = applicationContext;
        }
        public IActionResult Profile(string Id)
        {
            var user = (from Users in db.Users
                      where Users.UserName == Id
                      select Users).First();
            var matches = db.Matches
                .Include(m => m.Users )
                .ThenInclude(mu => mu.Match)
                .Where(x => x.Users.Any(c => c.UserId == user.Id))
                .ToList();

            return View(new ProfileViewModel() { User = user, Matches = matches });
        }
        [HttpGet]
        public IActionResult Register()
        {
            if(User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            else
                return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email, YearBorn = model.YearBorn };
                _userManager.Options.Password.RequireNonAlphanumeric = false;
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}