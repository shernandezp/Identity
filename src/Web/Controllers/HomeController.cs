using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }
}
