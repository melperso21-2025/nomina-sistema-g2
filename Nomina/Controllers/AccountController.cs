using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using Nomina.Models;

namespace Nomina.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;

        public AccountController(IConfiguration config)
        {
            _config = config;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            // Si ya hay sesion activa, ir al dashboard
            if (HttpContext.Session.GetString("usuario") != null)
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Usuario y contraseña son requeridos.";
                return View();
            }

            // Generar hash SHA256 de la contraseña
            byte[] passwordHash = SHA256.HashData(Encoding.UTF8.GetBytes(password));

            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_login", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@p_username", username);
                    cmd.Parameters.AddWithValue("@p_password_hash", passwordHash);

                    SqlParameter pResult   = new SqlParameter("@r_result",    System.Data.SqlDbType.Int)          { Direction = System.Data.ParameterDirection.Output };
                    SqlParameter pRole     = new SqlParameter("@r_role",      System.Data.SqlDbType.VarChar, 50)  { Direction = System.Data.ParameterDirection.Output };
                    SqlParameter pEmpNo    = new SqlParameter("@r_emp_no",    System.Data.SqlDbType.Int)          { Direction = System.Data.ParameterDirection.Output };
                    SqlParameter pFullName = new SqlParameter("@r_full_name", System.Data.SqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Output };

                    cmd.Parameters.Add(pResult);
                    cmd.Parameters.Add(pRole);
                    cmd.Parameters.Add(pEmpNo);
                    cmd.Parameters.Add(pFullName);

                    cmd.ExecuteNonQuery();

                    int result = (int)pResult.Value;

                    if (result == 1)
                    {
                        HttpContext.Session.SetString("usuario",   pFullName.Value.ToString());
                        HttpContext.Session.SetString("rol",       pRole.Value.ToString());
                        HttpContext.Session.SetInt32("emp_no",     (int)pEmpNo.Value);

                        RegistrarActividad("Auth", "LOGIN", $"Inicio de sesión: {username}");
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        ViewBag.Error = "Usuario o contraseña incorrectos.";
                        return View();
                    }
                }
            }
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            string nombreUsuario = HttpContext.Session.GetString("usuario");
            RegistrarActividad("Auth", "LOGOUT", $"Cierre de sesión: {nombreUsuario}");
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private void RegistrarActividad(string module, string action, string description = null)
        {
            try
            {
                string user    = HttpContext.Session.GetString("usuario") ?? "sistema";
                string connStr = _config.GetConnectionString("NominaDB");
                using SqlConnection conn = new SqlConnection(connStr);
                conn.Open();
                using SqlCommand cmd = new SqlCommand(
                    "INSERT INTO activity_log (user_session, module, action, description) VALUES (@u, @m, @a, @d)", conn);
                cmd.Parameters.AddWithValue("@u", user);
                cmd.Parameters.AddWithValue("@m", module);
                cmd.Parameters.AddWithValue("@a", action);
                cmd.Parameters.AddWithValue("@d", (object)description ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
            catch { /* No bloquear flujo principal */ }
        }
    }
}
