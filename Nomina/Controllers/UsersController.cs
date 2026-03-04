using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Nomina.Models;
using System.Security.Cryptography;
using System.Text;

namespace Nomina.Controllers
{
    public class UsersController : Controller
    {
        private readonly IConfiguration _config;

        public UsersController(IConfiguration config)
        {
            _config = config;
        }

        private bool VerificarSesion() =>
            HttpContext.Session.GetString("usuario") != null;

        private bool EsAdmin() =>
            HttpContext.Session.GetString("rol") == "Admin";

        // GET: /Users
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

            ViewBag.Usuario = HttpContext.Session.GetString("usuario");
            ViewBag.Rol     = HttpContext.Session.GetString("rol");
            return View(usuarios);
        }

        // GET: /Users/Crear
        public IActionResult Crear()
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (!EsAdmin())
            {
                TempData["Error"] = "Acceso restringido: se requiere rol Admin.";
                return RedirectToAction("Index");
            }

            CargarEmpleadosSinUsuario();
            return View();
        }

        // POST: /Users/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(int empNo, string username, string password, string rol)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (!EsAdmin())
            {
                TempData["Error"] = "Acceso restringido: se requiere rol Admin.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "El usuario y la contraseña son requeridos.";
                CargarEmpleadosSinUsuario();
                return View();
            }

            // Validar formato de username (solo letras, puntos y números)
            if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-z0-9.]+$"))
            {
                ViewBag.Error = "El nombre de usuario debe contener solo letras minúsculas, números y puntos.";
                CargarEmpleadosSinUsuario();
                return View();
            }

            byte[] hash    = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            string connStr = _config.GetConnectionString("NominaDB");

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    using (SqlCommand check = new SqlCommand(
                        "SELECT COUNT(*) FROM users WHERE username = @u", conn))
                    {
                        check.Parameters.AddWithValue("@u", username);
                        if ((int)check.ExecuteScalar() > 0)
                        {
                            ViewBag.Error = "El nombre de usuario ya está en uso. Intenta con otro (ej: nombre.apellido2).";
                            CargarEmpleadosSinUsuario();
                            return View();
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(
                        "INSERT INTO users (emp_no, username, password_hash, role) " +
                        "VALUES (@emp_no, @username, @hash, @role)", conn))
                    {
                        cmd.Parameters.AddWithValue("@emp_no",   empNo);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@hash",     hash);
                        cmd.Parameters.AddWithValue("@role",     rol);
                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["Exito"] = $"Usuario '{username}' creado correctamente.";
                return RedirectToAction("Index");
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                ViewBag.Error = "El nombre de usuario ya está en uso.";
                CargarEmpleadosSinUsuario();
                return View();
            }
        }

        // GET: /Users/Editar/5
        public IActionResult Editar(int empNo)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (!EsAdmin())
            {
                TempData["Error"] = "Acceso restringido: se requiere rol Admin.";
                return RedirectToAction("Index");
            }

            User? usuario = CargarUsuario(empNo);
            if (usuario == null) return NotFound();

            ViewBag.FullName = CargarNombreEmpleado(empNo);
            return View(usuario);
        }

        // POST: /Users/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int empNo, string username, string newPassword, string rol)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (!EsAdmin())
            {
                TempData["Error"] = "Acceso restringido: se requiere rol Admin.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                ViewBag.Error    = "El nombre de usuario es requerido.";
                ViewBag.FullName = CargarNombreEmpleado(empNo);
                return View(CargarUsuario(empNo));
            }

            string connStr = _config.GetConnectionString("NominaDB");

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    using (SqlCommand check = new SqlCommand(
                        "SELECT COUNT(*) FROM users WHERE username = @u AND emp_no <> @id", conn))
                    {
                        check.Parameters.AddWithValue("@u",  username);
                        check.Parameters.AddWithValue("@id", empNo);
                        if ((int)check.ExecuteScalar() > 0)
                        {
                            ViewBag.Error    = "El nombre de usuario ya está en uso.";
                            ViewBag.FullName = CargarNombreEmpleado(empNo);
                            return View(CargarUsuario(empNo));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(newPassword))
                    {
                        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(newPassword));
                        using (SqlCommand cmd = new SqlCommand(
                            "UPDATE users SET username = @u, role = @r, password_hash = @h " +
                            "WHERE emp_no = @id", conn))
                        {
                            cmd.Parameters.AddWithValue("@u",  username);
                            cmd.Parameters.AddWithValue("@r",  rol);
                            cmd.Parameters.AddWithValue("@h",  hash);
                            cmd.Parameters.AddWithValue("@id", empNo);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "UPDATE users SET username = @u, role = @r WHERE emp_no = @id", conn))
                        {
                            cmd.Parameters.AddWithValue("@u",  username);
                            cmd.Parameters.AddWithValue("@r",  rol);
                            cmd.Parameters.AddWithValue("@id", empNo);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                TempData["Exito"] = $"Usuario '{username}' actualizado correctamente.";
                return RedirectToAction("Index");
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                ViewBag.Error    = "El nombre de usuario ya está en uso.";
                ViewBag.FullName = CargarNombreEmpleado(empNo);
                return View(CargarUsuario(empNo));
            }
        }

        // POST: /Users/Eliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int empNo)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (!EsAdmin())
            {
                TempData["Error"] = "Acceso restringido: se requiere rol Admin.";
                return RedirectToAction("Index");
            }

            int currentEmpNo = HttpContext.Session.GetInt32("emp_no") ?? -1;
            if (currentEmpNo == empNo)
            {
                TempData["Error"] = "No puedes eliminar tu propio usuario.";
                return RedirectToAction("Index");
            }

            string connStr = _config.GetConnectionString("NominaDB");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                using (SqlCommand roleCmd = new SqlCommand(
                    "SELECT role FROM users WHERE emp_no = @id", conn))
                {
                    roleCmd.Parameters.AddWithValue("@id", empNo);
                    string targetRole = roleCmd.ExecuteScalar()?.ToString() ?? "";

                    if (targetRole == "Admin")
                    {
                        using (SqlCommand countCmd = new SqlCommand(
                            "SELECT COUNT(*) FROM users WHERE role = 'Admin'", conn))
                        {
                            if ((int)countCmd.ExecuteScalar() <= 1)
                            {
                                TempData["Error"] = "No se puede eliminar el único administrador del sistema.";
                                return RedirectToAction("Index");
                            }
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(
                    "DELETE FROM users WHERE emp_no = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", empNo);
                    cmd.ExecuteNonQuery();
                }
            }

            TempData["Exito"] = "Usuario eliminado correctamente.";
            return RedirectToAction("Index");
        }

        // ─── HELPERS ─────────────────────────────────────────────────────────

        private void CargarEmpleadosSinUsuario()
        {
            string connStr = _config.GetConnectionString("NominaDB");
            var lista = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT e.emp_no, e.first_name + ' ' + e.last_name AS full_name " +
                    "FROM employees e " +
                    "WHERE NOT EXISTS (SELECT 1 FROM users u WHERE u.emp_no = e.emp_no) " +
                    "ORDER BY e.last_name, e.first_name", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        lista.Add(new { EmpNo = reader.GetInt32(0), FullName = reader.GetString(1) });
                }
            }

            ViewBag.Empleados = lista;
        }

        private User? CargarUsuario(int empNo)
        {
            string connStr = _config.GetConnectionString("NominaDB");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT emp_no, username, role FROM users WHERE emp_no = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", empNo);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return new User
                            {
                                EmpNo    = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Rol      = reader.GetString(2)
                            };
                    }
                }
            }
            return null;
        }

        private string CargarNombreEmpleado(int empNo)
        {
            string connStr = _config.GetConnectionString("NominaDB");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT first_name + ' ' + last_name FROM employees WHERE emp_no = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", empNo);
                    return cmd.ExecuteScalar()?.ToString() ?? "—";
                }
            }
        }
    }
}
