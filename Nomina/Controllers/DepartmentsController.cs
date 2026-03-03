using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Nomina.Models;

namespace Nomina.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IConfiguration _config;

        public DepartmentsController(IConfiguration config)
        {
            _config = config;
        }

        private bool VerificarSesion() =>
            HttpContext.Session.GetString("usuario") != null;

        // GET: /Departments
        public IActionResult Index(string searchString = null, int page = 1, string sortBy = "codigo", string sortDirection = "asc")
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            var departamentos = new List<Department>();
            int total    = 0;
            int pageSize = 20;
            string connStr = _config.GetConnectionString("NominaDB");

            const string baseFrom = @"
                FROM departments
                WHERE is_active = 1
                  AND (@search IS NULL
                       OR dept_name LIKE '%' + @search + '%')";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                using (SqlCommand countCmd = new SqlCommand("SELECT COUNT(*) " + baseFrom, conn))
                {
                    countCmd.Parameters.AddWithValue("@search",
                        string.IsNullOrWhiteSpace(searchString) ? (object)DBNull.Value : searchString);
                    total = Convert.ToInt32(countCmd.ExecuteScalar());
                }

                // Determinar el campo y dirección de ordenamiento
                string orderByClause = "";
                switch (sortBy.ToLower())
                {
                    case "codigo":
                        // Para ordenar numéricamente cuando dept_no son números
                        orderByClause = $"ORDER BY CAST(dept_no AS INT) {(sortDirection.ToLower() == "desc" ? "DESC" : "ASC")}";
                        break;
                    case "nombre":
                        orderByClause = $"ORDER BY dept_name {(sortDirection.ToLower() == "desc" ? "DESC" : "ASC")}";
                        break;
                    default:
                        orderByClause = "ORDER BY CAST(dept_no AS INT) ASC";
                        break;
                }

                string dataSql = "SELECT dept_no, dept_name, is_active " + baseFrom + @"
                    " + orderByClause + @"
                    OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                using (SqlCommand cmd = new SqlCommand(dataSql, conn))
                {
                    cmd.Parameters.AddWithValue("@search",
                        string.IsNullOrWhiteSpace(searchString) ? (object)DBNull.Value : searchString);
                    cmd.Parameters.AddWithValue("@offset",   (page - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            departamentos.Add(new Department
                            {
                                DeptNo   = reader.GetValue(0).ToString(),
                                DeptName = reader.GetValue(1).ToString(),
                                IsActive = Convert.ToBoolean(reader.GetValue(2))
                            });
                        }
                    }
                }
            }

            ViewBag.Total        = total;
            ViewBag.Page         = page;
            ViewBag.SearchString = searchString;
            ViewBag.SortBy       = sortBy;
            ViewBag.SortDirection = sortDirection;
            ViewBag.Usuario      = HttpContext.Session.GetString("usuario");
            ViewBag.Rol          = HttpContext.Session.GetString("rol");

            return View(departamentos);
        }

        // GET: /Departments/Detalle/d001
        public IActionResult Detalle(string id)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            Department dept = null;
            int empleadosActivos = 0;
            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
                    SELECT d.dept_no, d.dept_name, d.is_active,
                           COUNT(de.emp_no) AS empleados_activos
                    FROM departments d
                    LEFT JOIN dept_emp de ON d.dept_no = de.dept_no AND de.to_date IS NULL
                    WHERE d.dept_no = @deptNo
                    GROUP BY d.dept_no, d.dept_name, d.is_active";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@deptNo", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dept = new Department
                            {
                                DeptNo   = reader.GetValue(0).ToString(),
                                DeptName = reader.GetValue(1).ToString(),
                                IsActive = Convert.ToBoolean(reader.GetValue(2))
                            };
                            empleadosActivos = Convert.ToInt32(reader.GetValue(3));
                        }
                    }
                }
            }

            if (dept == null)
                return NotFound();

            ViewBag.EmpleadosActivos = empleadosActivos;
            ViewBag.Usuario          = HttpContext.Session.GetString("usuario");
            ViewBag.Rol              = HttpContext.Session.GetString("rol");

            return View(dept);
        }

        // GET: /Departments/Crear
        public IActionResult Crear()
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("rol") != "Admin")
            {
                TempData["Error"] = "Acceso denegado. Solo administradores.";
                return RedirectToAction("Index");
            }

            string connStr = _config.GetConnectionString("NominaDB");
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new SqlCommand(
                    "SELECT ISNULL(MAX(CAST(dept_no AS INT)), 0) FROM departments", conn))
                {
                    int max = Convert.ToInt32(cmd.ExecuteScalar());
                    ViewBag.NextDeptNo = (max + 1).ToString();
                }
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(string dept_no, string dept_name)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("rol") != "Admin")
            {
                TempData["Error"] = "Acceso denegado. Solo administradores.";
                return RedirectToAction("Index");
            }

            string connStr = _config.GetConnectionString("NominaDB");
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_insert_department", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_dept_no",   dept_no);
                        cmd.Parameters.AddWithValue("@p_dept_name", dept_name);

                        SqlParameter pMsg = new SqlParameter("@r_message", System.Data.SqlDbType.VarChar, 200)
                        { Direction = System.Data.ParameterDirection.Output };
                        cmd.Parameters.Add(pMsg);

                        cmd.ExecuteNonQuery();

                        string msg = pMsg.Value?.ToString() ?? string.Empty;
                        if (msg.StartsWith("SUCCESS"))
                        {
                            RegistrarActividad("Departments", "CREATE", $"Departamento creado: {dept_no} - {dept_name}");
                            TempData["Exito"] = "Departamento creado correctamente.";
                            return RedirectToAction("Index");
                        }

                        ViewBag.Error = msg.Replace("ERROR: ", "");
                        return View();
                    }
                }
            }
            catch (SqlException ex)
            {
                ViewBag.Error = $"Error de base de datos: {ex.Message}";
                return View();
            }
        }

        // GET: /Departments/Editar/d001
        public IActionResult Editar(string id)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("rol") != "Admin")
            {
                TempData["Error"] = "Acceso denegado. Solo administradores.";
                return RedirectToAction("Index");
            }

            Department dept = null;
            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT dept_no, dept_name, is_active FROM departments WHERE dept_no = @deptNo", conn))
                {
                    cmd.Parameters.AddWithValue("@deptNo", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dept = new Department
                            {
                                DeptNo   = reader.GetValue(0).ToString(),
                                DeptName = reader.GetValue(1).ToString(),
                                IsActive = Convert.ToBoolean(reader.GetValue(2))
                            };
                        }
                    }
                }
            }

            if (dept == null)
                return NotFound();

            ViewBag.Usuario = HttpContext.Session.GetString("usuario");
            return View(dept);
        }

        // POST: /Departments/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(string dept_no, string dept_name)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("rol") != "Admin")
            {
                TempData["Error"] = "Acceso denegado. Solo administradores.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(dept_name))
            {
                ViewBag.Error = "El nombre del departamento es requerido.";
                return View(new Department { DeptNo = dept_no, DeptName = dept_name });
            }

            string connStr = _config.GetConnectionString("NominaDB");
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_update_department", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_dept_no",   dept_no);
                        cmd.Parameters.AddWithValue("@p_dept_name", dept_name);

                        SqlParameter pMsg = new SqlParameter("@r_message", System.Data.SqlDbType.VarChar, 200)
                        { Direction = System.Data.ParameterDirection.Output };
                        cmd.Parameters.Add(pMsg);

                        cmd.ExecuteNonQuery();

                        string msg = pMsg.Value?.ToString() ?? string.Empty;
                        if (msg.StartsWith("SUCCESS"))
                        {
                            RegistrarActividad("Departments", "UPDATE", $"Departamento actualizado: {dept_no} - {dept_name}");
                            TempData["Exito"] = "Departamento actualizado correctamente.";
                            return RedirectToAction("Index");
                        }

                        ViewBag.Error = msg.Replace("ERROR: ", "");
                        return View(new Department { DeptNo = dept_no, DeptName = dept_name });
                    }
                }
            }
            catch (SqlException ex)
            {
                ViewBag.Error = $"Error de base de datos: {ex.Message}";
                return View(new Department { DeptNo = dept_no, DeptName = dept_name });
            }
        }

        // POST: /Departments/Desactivar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Desactivar(string id)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("rol") != "Admin")
            {
                TempData["Error"] = "Acceso denegado. Solo administradores.";
                return RedirectToAction("Index");
            }

            string connStr = _config.GetConnectionString("NominaDB");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE departments SET is_active = 0 WHERE dept_no = @deptNo", conn))
                {
                    cmd.Parameters.AddWithValue("@deptNo", id);
                    cmd.ExecuteNonQuery();
                }
            }

            RegistrarActividad("Departments", "DEACTIVATE", $"Departamento desactivado: {id}");
            TempData["Exito"] = "Departamento desactivado correctamente.";
            return RedirectToAction("Index");
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
