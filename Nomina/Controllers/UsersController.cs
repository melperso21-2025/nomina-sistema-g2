using Microsoft.AspNetCore.Mvc;

namespace Nomina.Controllers;

public class UsersController : Controller
{
    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("usuario") == null)
            return RedirectToAction("Login", "Account");

        // Solo Admin puede acceder
        if (HttpContext.Session.GetString("rol") != "Admin")
            return RedirectToAction("Index", "Dashboard");

        return View();
    }
}