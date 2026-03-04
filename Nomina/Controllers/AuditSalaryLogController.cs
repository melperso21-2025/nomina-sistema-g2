using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Nomina.Models;

namespace Nomina.Controllers
{
    public class AuditSalaryLogController : Controller
    {
        private readonly IConfiguration _config;

        public AuditSalaryLogController(IConfiguration config)
        {
            _config = config;
        }

        private bool VerificarSesion() =>
            HttpContext.Session.GetString("usuario") != null;

        // GET: /Auditoria
        public IActionResult Index(bool showInactive = false)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            var registros = new List<LogAuditory>();
            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT TOP 100 e.emp_no, sal.action_date, sal.previous_salary, sal.new_salary, sal.from_date, " +
                    "RTRIM(CONVERT(NVARCHAR(200), sal.user_session)) AS user_session, e.is_active " +
                    "FROM salary_audit_log sal " +
                    "INNER JOIN employees e ON sal.emp_no = e.emp_no " +
                    (showInactive ? "" : "WHERE e.is_active = 1 ") +
                    "ORDER BY sal.action_date DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            registros.Add(new LogAuditory
                                {
                                    EmpNo            = reader.GetInt32(0),
                                    ActionDate       = reader.GetDateTime(1),
                                    SalarioAnterior  = reader.IsDBNull(2) ? null : Convert.ToDecimal(reader.GetValue(2)),
                                    SalarioNuevo     = Convert.ToDecimal(reader.GetValue(3)),
                                    FechaCambio      = reader.GetDateTime(4),
                                    UserResponsable  = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                    IsActive         = Convert.ToBoolean(reader.GetValue(6))
                                });
                        }
                    }
                }
            }

            ViewBag.Usuario = HttpContext.Session.GetString("usuario");
            ViewBag.Rol     = HttpContext.Session.GetString("rol");
            ViewBag.ShowInactive = showInactive;
            return View(registros);
        }
    }
}
