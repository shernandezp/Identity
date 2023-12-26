using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Security.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Security.Application.Users.Queries.GetUsers;

namespace Web.Controllers;
public class LoginController(ISender sender) : Controller
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

        if (!ModelState.IsValid)
            return View(model);

        var user = await sender.Send(new GetUsersQuery(model.Email, model.Password));
        if (!string.IsNullOrEmpty(user.Username))
        {
            var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.Username)
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
