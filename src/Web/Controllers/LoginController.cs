using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Security.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers;
public class LoginController : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(LoginViewModel model)
    {
        ViewData["ReturnUrl"] = model.ReturnUrl;

        if (ModelState.IsValid)
        {
            var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, model.Username)
                };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

            return Redirect(model.ReturnUrl);
        }

        return View(model);
    }

    //public async Task<IActionResult> Logout()
    public async Task Logout()
    {
        await HttpContext.SignOutAsync();

        //return RedirectToAction(nameof(HomeController.Index), "Home");
    }
}
