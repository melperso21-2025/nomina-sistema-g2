using Microsoft.AspNetCore.Mvc;

namespace Nomina.Controllers;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetString("usuario") != null)
            return RedirectToAction("Index", "Dashboard");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(string usuario, string clave)
    {
        // TODO: validar contra base de datos
        if (usuario == "admin" && clave == "1234")
        {
            HttpContext.Session.SetString("usuario", usuario);
            HttpContext.Session.SetString("nombre", "Administrador");
            HttpContext.Session.SetString("rol", "Admin");
            return RedirectToAction("Index", "Dashboard");
        }

        ViewBag.Error = "Usuario o contraseña incorrectos.";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}