using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using LibraryWebApplication.Models;
using LibraryWebApplication.ViewModel;
using Microsoft.AspNetCore.Authorization;


namespace LibraryWebApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email, Year = model.Year };
                //add user
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Action(
                    //    "ConfirmEmail",
                    //    "Account",
                    //    new { userId = user.Id, code = code },
                    //    protocol: HttpContext.Request.Scheme);
                    //EmailService emailService = new EmailService();
                    //await emailService.SendEmailAsync(model.Email, "Confirm your account",
                    //    $"Підтвердіть реєстрацію: <a href='{callbackUrl}'>link</a>");
                    //download cookies
                    await _signInManager.SignInAsync(user, true);
                    if (await _roleManager.FindByNameAsync("user") == null)
                    {
                        await _roleManager.CreateAsync(new IdentityRole("user"));
                    }
                    await _userManager.AddToRoleAsync(user, "user");
                    //return Content("Для завершення реєстрації перевірте електронну пошту та перейдіть за посиланням, указаному в листі");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        //public async Task<IActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return View("Error");
        //    }
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        return View("Error");
        //    }
        //    var result = await _userManager.ConfirmEmailAsync(user, code);
        //    if (result.Succeeded)
        //    {
        //        await _signInManager.SignInAsync(user, true);
        //        if (await _roleManager.FindByNameAsync("user") == null)
        //        {
        //            await _roleManager.CreateAsync(new IdentityRole("user"));
        //        }
        //        await _userManager.AddToRoleAsync(user, "user");
        //        return RedirectToAction("Index", "Home");
        //    }
        //    else
        //        return View("Error");
        //}

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                //var user = await _userManager.FindByNameAsync(model.Email);
                //if (user != null)
                //{
                //    проверяем, подтвержден ли email
                //    if (!await _userManager.IsEmailConfirmedAsync(user))
                //    {
                //        ModelState.AddModelError(string.Empty, "Ви не підтвердили свій email");
                //        return View(model);
                //    }
                //}

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // перевіряємо, чи належить URL додатку
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
                    ModelState.AddModelError("", "Невiрний логiн або пароль!");
                }
                
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            //видаляємо аутентифікаційні куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
