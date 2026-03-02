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

            if (HttpContext.Session.GetString("rol") != "Admin")
            {
                TempData["Error"] = "Acceso denegado. Información confidencial.";
                return RedirectToAction("Index", "Home");
            }

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

            if (HttpContext.Session.GetString("rol") != "Admin")
            {
                TempData["Error"] = "Acceso denegado. Información confidencial.";
                return RedirectToAction("Index", "Home");
            }

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
                                Salary      = Convert.ToDecimal(reader.GetValue(5)),
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

            if (HttpContext.Session.GetString("rol") != "Admin")
            {
                TempData["Error"] = "Acceso denegado. Información confidencial.";
                return RedirectToAction("Index", "Home");
            }

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
                                PreviousSalary = reader.IsDBNull(5) ? 0m : Convert.ToDecimal(reader.GetValue(5)),
                                NewSalary      = Convert.ToDecimal(reader.GetValue(6)),
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

        // GET: /Reportes/EstructuraOrganizacional
        public IActionResult EstructuraOrganizacional()
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("rol") != "Admin")
            {
                TempData["Error"] = "Acceso denegado. Información confidencial.";
                return RedirectToAction("Index", "Home");
            }

            var resultados = new List<OrgChartItem>();
            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
                    SELECT d.dept_name,
                           ISNULL(mgr.first_name + ' ' + mgr.last_name, 'Sin gerente') AS manager_name,
                           COUNT(de.emp_no) AS employee_count
                    FROM departments d
                    LEFT JOIN dept_manager dm  ON d.dept_no = dm.dept_no  AND dm.to_date IS NULL
                    LEFT JOIN employees   mgr  ON dm.emp_no = mgr.emp_no
                    LEFT JOIN dept_emp    de   ON d.dept_no = de.dept_no  AND de.to_date IS NULL
                    WHERE d.is_active = 1
                    GROUP BY d.dept_name, mgr.first_name, mgr.last_name
                    ORDER BY d.dept_name";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        resultados.Add(new OrgChartItem
                        {
                            DeptName      = reader.GetString(0),
                            ManagerName   = reader.GetString(1),
                            EmployeeCount = Convert.ToInt32(reader.GetValue(2))
                        });
                    }
                }
            }

            ViewBag.Usuario = HttpContext.Session.GetString("usuario");
            ViewBag.Rol     = HttpContext.Session.GetString("rol");
            return View(resultados);
        }

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
                            DeptNo = reader.GetValue(0).ToString(),
                            DeptName = reader.GetString(1)
                        });
                    }
                }
            }

            ViewBag.Departamentos = departamentos;
        }
    }
}
