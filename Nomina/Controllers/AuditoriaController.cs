using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Nomina.Models;

namespace Nomina.Controllers
{
    public class AuditoriaController : Controller
    {
        private readonly IConfiguration _config;

        public AuditoriaController(IConfiguration config)
        {
            _config = config;
        }

        private bool VerificarSesion() =>
            HttpContext.Session.GetString("usuario") != null;

        // GET: /Auditoria
        public IActionResult Index()
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            var registros = new List<LogAuditory>();
            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT TOP 100 emp_no, action_date, previus_salary, new_salary, user_session " +
                    "FROM salary_audit_log ORDER BY action_date DESC", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            registros.Add(new LogAuditory
                            {
                                EmpNo            = reader.GetInt32(0),
                                ActionDate       = reader.GetDateTime(1),
                                SalarioAnterior  = reader.IsDBNull(2) ? null : (decimal?)reader.GetDecimal(2),
                                SalarioNuevo     = reader.GetDecimal(3),
                                UserResponsable  = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                            });
                        }
                    }
                }
            }

            ViewBag.Usuario = HttpContext.Session.GetString("usuario");
            ViewBag.Rol     = HttpContext.Session.GetString("rol");
            return View(registros);
        }
    }
}
