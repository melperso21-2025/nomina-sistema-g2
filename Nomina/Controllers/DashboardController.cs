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

        private bool VerificarSesion() =>
            HttpContext.Session.GetString("usuario") != null;

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

                // KPI 1: Total empleados
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM dbo.employees", conn))
                    stats.TotalEmpleados = (int)cmd.ExecuteScalar();

                // KPI 2: Total departamentos
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM dbo.departments", conn))
                    stats.TotalDepartamentos = (int)cmd.ExecuteScalar();

                // KPI 3: Salario promedio (solo registros activos)
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT ISNULL(AVG(CAST(salary AS BIGINT)), 0) FROM dbo.salaries WHERE to_date IS NULL", conn))
                    stats.SalarioPromedio = Convert.ToInt64(cmd.ExecuteScalar());

                // Últimos 10 cambios de salario
                // CONVERT(NVARCHAR) fuerza la decodificación correcta de caracteres especiales
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT TOP 10 " +
                    "CONVERT(NVARCHAR(200), e.first_name) + ' ' + CONVERT(NVARCHAR(200), e.last_name) AS full_name, " +
                    "sal.previous_salary, " +
                    "sal.new_salary, " +
                    "sal.action_date, " +
                    "RTRIM(CONVERT(NVARCHAR(200), sal.user_session)) AS user_session, " +
                    "e.is_active " +
                    "FROM salary_audit_log sal " +
                    "INNER JOIN employees e ON sal.emp_no = e.emp_no " +
                    "ORDER BY sal.action_date DESC", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stats.UltimosCambios.Add(new SalaryChangeItem
                        {
                            FullName       = reader.GetString(0),
                            PreviousSalary = reader.IsDBNull(1) ? 0L : Convert.ToInt64(reader.GetValue(1)),
                            NewSalary      = Convert.ToInt64(reader.GetValue(2)),
                            ActionDate     = reader.GetDateTime(3),
                            UserSession    = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            IsActive       = Convert.ToBoolean(reader.GetValue(5))
                        });
                    }
                }
            }

            ViewBag.Usuario = HttpContext.Session.GetString("usuario");
            ViewBag.Rol     = HttpContext.Session.GetString("rol");
            return View(stats);
        }
    }
}
