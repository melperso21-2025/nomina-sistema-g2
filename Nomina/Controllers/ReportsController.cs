using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Nomina.Models;

namespace Nomina.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IConfiguration _config;

        public ReportsController(IConfiguration config)
        {
            _config = config;
        }

        private bool VerificarSesion()
        {
            return HttpContext.Session.GetString("usuario") != null;
        }

        // GET: /Reportes
        public IActionResult Index()
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            CargarDepartamentos();
            ViewBag.Usuario = HttpContext.Session.GetString("usuario");
            ViewBag.Rol     = HttpContext.Session.GetString("rol");
            return View();
        }

        // GET: /Reportes/NominaVigente
        public IActionResult NominaVigente(string deptNo)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            var resultados = new List<PayrollReportItem>();
            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_report_active_payroll", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_dept_no", string.IsNullOrEmpty(deptNo) ? (object)DBNull.Value : deptNo);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultados.Add(new PayrollReportItem
                            {
                                DeptName    = reader.GetString(0),
                                EmpNo       = reader.GetInt32(1),
                                FullName    = reader.GetString(2),
                                Ci          = reader.GetString(3),
                                Title       = reader.IsDBNull(4) ? "Sin cargo" : reader.GetString(4),
                                Salary      = reader.GetInt64(5),
                                SalarySince = reader.GetDateTime(6)
                            });
                        }
                    }
                }
            }

            CargarDepartamentos();
            ViewBag.DeptNoSeleccionado = deptNo;
            ViewBag.Usuario = HttpContext.Session.GetString("usuario");
            ViewBag.Rol     = HttpContext.Session.GetString("rol");
            return View(resultados);
        }

        // GET: /Reportes/CambiosSalariales
        public IActionResult CambiosSalariales(DateTime? dateFrom, DateTime? dateTo, string ci)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            var resultados = new List<SalaryChangesReportItem>();
            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_report_salary_changes", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_date_from", dateFrom.HasValue ? (object)dateFrom.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_date_to",   dateTo.HasValue   ? (object)dateTo.Value   : DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_ci",        string.IsNullOrEmpty(ci) ? (object)DBNull.Value : ci);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultados.Add(new SalaryChangesReportItem
                            {
                                LogId          = reader.GetInt32(0),
                                ActionDate     = reader.GetDateTime(1),
                                UserSession    = reader.GetString(2),
                                FullName       = reader.GetString(3),
                                Ci             = reader.GetString(4),
                                PreviousSalary = reader.IsDBNull(5) ? 0 : reader.GetInt64(5),
                                NewSalary      = reader.GetInt64(6),
                                FromDate       = reader.GetDateTime(7)
                            });
                        }
                    }
                }
            }

            ViewBag.DateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.DateTo   = dateTo?.ToString("yyyy-MM-dd");
            ViewBag.Ci       = ci;
            ViewBag.Usuario  = HttpContext.Session.GetString("usuario");
            ViewBag.Rol      = HttpContext.Session.GetString("rol");
            return View(resultados);
        }

        // ─── HELPERS ────────────────────────────────────────────

        private void CargarDepartamentos()
        {
            var departamentos = new List<DepartmentItem>();
            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT dept_no, dept_name FROM departments WHERE is_active = 1 ORDER BY dept_name", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        departamentos.Add(new DepartmentItem
                        {
                            DeptNo   = reader.GetString(0),
                            DeptName = reader.GetString(1)
                        });
                    }
                }
            }

            ViewBag.Departamentos = departamentos;
        }
    }
}
