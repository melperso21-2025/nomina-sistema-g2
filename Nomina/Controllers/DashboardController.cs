using Microsoft.AspNetCore.Mvc;

namespace Nomina.Controllers;

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("usuario") == null)
            return RedirectToAction("Login", "Account");

        return View();
    }
}