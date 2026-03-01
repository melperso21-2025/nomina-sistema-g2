using Microsoft.AspNetCore.Mvc;

namespace Nomina.Controllers;

public class EmployeesController : Controller
{
    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("usuario") == null)
            return RedirectToAction("Login", "Account");

        return View();
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (HttpContext.Session.GetString("usuario") == null)
            return RedirectToAction("Login", "Account");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(string ci, string first_name, string last_name,
        string correo, string departamento, string hire_date, string gender)
    {
        // TODO: guardar en base de datos
        TempData["Exito"] = "Empleado creado correctamente.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        if (HttpContext.Session.GetString("usuario") == null)
            return RedirectToAction("Login", "Account");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, string ci, string first_name, string last_name,
        string correo, string departamento, string hire_date, string gender)
    {
        // TODO: actualizar en base de datos
        TempData["Exito"] = "Empleado actualizado correctamente.";
        return RedirectToAction("Index");
    }
}