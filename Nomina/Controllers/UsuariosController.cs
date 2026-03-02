using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Nomina.Models;

namespace Nomina.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IConfiguration _config;

        public UsuariosController(IConfiguration config)
        {
            _config = config;
        }

        private bool VerificarSesion() =>
            HttpContext.Session.GetString("usuario") != null;

        private bool EsAdmin() =>
            HttpContext.Session.GetString("rol") == "Admin";

        // GET: /Usuarios
        public IActionResult Index()
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (!EsAdmin())
            {
                TempData["Error"] = "Acceso restringido: se requiere rol Admin.";
                return RedirectToAction("Index", "Dashboard");
            }

            var usuarios = new List<User>();
            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT emp_no, username, role FROM users ORDER BY username", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usuarios.Add(new User
                            {
                                EmpNo    = reader.IsDBNull(0) ? null : reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Rol      = reader.GetString(2)
                            });
                        }
                    }
                }
            }

            ViewBag.Usuario = HttpContext.Session.GetString("usuario");
            ViewBag.Rol     = HttpContext.Session.GetString("rol");
            return View(usuarios);
        }
    }
}
