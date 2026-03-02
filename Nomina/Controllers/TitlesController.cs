using Microsoft.AspNetCore.Mvc;

namespace Nomina.Controllers
{
    public class TitlesController : Controller
    {
        private bool VerificarSesion() =>
            HttpContext.Session.GetString("usuario") != null;

        // GET: /Titles
        public IActionResult Index()
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            ViewBag.Usuario = HttpContext.Session.GetString("usuario");
            ViewBag.Rol     = HttpContext.Session.GetString("rol");
            return View();
        }
    }
}
