using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Nomina.Models;
using OfficeOpenXml;

namespace Nomina.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IConfiguration _config;

        public ReportsController(IConfiguration config)
        {
            _config = config;
            // Configurar licencia de EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
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

            var resultados = ObtenerNominaVigente(deptNo);

            CargarDepartamentos();
            ViewBag.DeptNoSeleccionado = deptNo;
            ViewBag.Usuario = HttpContext.Session.GetString("usuario");
            ViewBag.Rol     = HttpContext.Session.GetString("rol");
            return View(resultados);
        }

        // GET: /Reportes/ExportarNominaExcel
        public IActionResult ExportarNominaExcel(string deptNo)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("rol") != "Admin")
                return Unauthorized();

            var datos = ObtenerNominaVigente(deptNo);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Nómina Vigente");

                // Headers
                worksheet.Cells[1, 1].Value = "Departamento";
                worksheet.Cells[1, 2].Value = "Emp #";
                worksheet.Cells[1, 3].Value = "Nombre Completo";
                worksheet.Cells[1, 4].Value = "Cédula";
                worksheet.Cells[1, 5].Value = "Cargo";
                worksheet.Cells[1, 6].Value = "Salario";
                worksheet.Cells[1, 7].Value = "Desde";

                // Formato de headers
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                }

                // Datos
                int row = 2;
                foreach (var item in datos)
                {
                    worksheet.Cells[row, 1].Value = item.DeptName;
                    worksheet.Cells[row, 2].Value = item.EmpNo;
                    worksheet.Cells[row, 3].Value = item.FullName;
                    worksheet.Cells[row, 4].Value = item.Ci;
                    worksheet.Cells[row, 5].Value = item.Title;
                    worksheet.Cells[row, 6].Value = item.Salary;
                    worksheet.Cells[row, 6].Style.Numberformat.Format = "$#,##0.00";
                    worksheet.Cells[row, 7].Value = item.SalarySince.ToString("yyyy-MM-dd");
                    row++;
                }

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream.ToArray(), 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Nomina_Vigente_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx");
            }
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

            var resultados = ObtenerCambiosSalariales(dateFrom, dateTo, ci);

            ViewBag.DateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.DateTo   = dateTo?.ToString("yyyy-MM-dd");
            ViewBag.Ci       = ci;
            ViewBag.Usuario  = HttpContext.Session.GetString("usuario");
            ViewBag.Rol      = HttpContext.Session.GetString("rol");
            return View(resultados);
        }

        // GET: /Reportes/ExportarCambiosExcel
        public IActionResult ExportarCambiosExcel(DateTime? dateFrom, DateTime? dateTo, string ci)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("rol") != "Admin")
                return Unauthorized();

            var datos = ObtenerCambiosSalariales(dateFrom, dateTo, ci);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Cambios Salariales");

                // Headers
                worksheet.Cells[1, 1].Value = "Fecha";
                worksheet.Cells[1, 2].Value = "Usuario";
                worksheet.Cells[1, 3].Value = "Nombre Completo";
                worksheet.Cells[1, 4].Value = "Cédula";
                worksheet.Cells[1, 5].Value = "Salario Anterior";
                worksheet.Cells[1, 6].Value = "Nuevo Salario";
                worksheet.Cells[1, 7].Value = "Diferencia";
                worksheet.Cells[1, 8].Value = "Desde";

                // Formato de headers
                using (var range = worksheet.Cells[1, 1, 1, 8])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                }

                // Datos
                int row = 2;
                foreach (var item in datos)
                {
                    worksheet.Cells[row, 1].Value = item.ActionDate.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cells[row, 2].Value = item.UserSession;
                    worksheet.Cells[row, 3].Value = item.FullName;
                    worksheet.Cells[row, 4].Value = item.Ci;
                    worksheet.Cells[row, 5].Value = item.PreviousSalary;
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "$#,##0.00";
                    worksheet.Cells[row, 6].Value = item.NewSalary;
                    worksheet.Cells[row, 6].Style.Numberformat.Format = "$#,##0.00";
                    worksheet.Cells[row, 7].Value = item.NewSalary - item.PreviousSalary;
                    worksheet.Cells[row, 7].Style.Numberformat.Format = "$#,##0.00";
                    worksheet.Cells[row, 8].Value = item.FromDate.ToString("yyyy-MM-dd");
                    row++;
                }

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream.ToArray(), 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Cambios_Salariales_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx");
            }
        }

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

            var resultados = ObtenerEstructuraOrganizacional();

            ViewBag.Usuario = HttpContext.Session.GetString("usuario");
            ViewBag.Rol     = HttpContext.Session.GetString("rol");
            return View(resultados);
        }

        // GET: /Reportes/ExportarEstructuraExcel
        public IActionResult ExportarEstructuraExcel()
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("rol") != "Admin")
                return Unauthorized();

            var datos = ObtenerEstructuraOrganizacional();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Estructura Organizacional");

                // Headers
                worksheet.Cells[1, 1].Value = "Departamento";
                worksheet.Cells[1, 2].Value = "Gerente";
                worksheet.Cells[1, 3].Value = "Cantidad de Empleados";

                // Formato de headers
                using (var range = worksheet.Cells[1, 1, 1, 3])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                }

                // Datos
                int row = 2;
                foreach (var item in datos)
                {
                    worksheet.Cells[row, 1].Value = item.DeptName;
                    worksheet.Cells[row, 2].Value = item.ManagerName;
                    worksheet.Cells[row, 3].Value = item.EmployeeCount;
                    row++;
                }

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream.ToArray(), 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Estructura_Organizacional_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx");
            }
        }

        // ─── MÉTODOS AUXILIARES ─────────────────────────────────

        private List<PayrollReportItem> ObtenerNominaVigente(string deptNo)
        {
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

            return resultados;
        }

        private List<SalaryChangesReportItem> ObtenerCambiosSalariales(DateTime? dateFrom, DateTime? dateTo, string ci)
        {
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

            return resultados;
        }

        private List<OrgChartItem> ObtenerEstructuraOrganizacional()
        {
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

            return resultados;
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
