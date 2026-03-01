using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Nomina.Models;

namespace Nomina.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IConfiguration _config;

        public DashboardController(IConfiguration config)
        {
            _config = config;
        }

        private bool VerificarSesion()
        {
            return HttpContext.Session.GetString("usuario") != null;
        }

        // GET: /Dashboard
        public IActionResult Index()
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            var stats = new DashboardViewModel();
            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_dashboard_stats", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Primer resultado: total empleados
                        if (reader.Read())
                            stats.TotalEmpleados = reader.GetInt32(0);

                        // Segundo resultado: total departamentos
                        if (reader.NextResult() && reader.Read())
                            stats.TotalDepartamentos = reader.GetInt32(0);

                        // Tercer resultado: salario promedio
                        if (reader.NextResult() && reader.Read())
                            stats.SalarioPromedio = reader.GetInt64(0);

                        // Cuarto resultado: ultimos cambios salariales
                        if (reader.NextResult())
                        {
                            stats.UltimosCambios = new List<SalaryChangeItem>();
                            while (reader.Read())
                            {
                                stats.UltimosCambios.Add(new SalaryChangeItem
                                {
                                    ActionDate      = reader.GetDateTime(0),
                                    FullName        = reader.GetString(1),
                                    PreviousSalary  = reader.IsDBNull(2) ? 0 : reader.GetInt64(2),
                                    NewSalary       = reader.GetInt64(3),
                                    UserSession     = reader.GetString(4)
                                });
                            }
                        }
                    }
                }
            }

            ViewBag.Usuario = HttpContext.Session.GetString("usuario");
            ViewBag.Rol     = HttpContext.Session.GetString("rol");
            return View(stats);
        }
    }
}
