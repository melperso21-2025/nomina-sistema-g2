using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Nomina.Models;

namespace Nomina.Controllers
{
    public class TitlesController : Controller
    {
        private readonly IConfiguration _config;

        public TitlesController(IConfiguration config)
        {
            _config = config;
        }

        private bool VerificarSesion() =>
            HttpContext.Session.GetString("usuario") != null;

        // GET: /Titles
        public IActionResult Index(string searchString = null, int page = 1, string sortBy = "cargo", string sortDirection = "asc")
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            var cargos   = new List<TitleListItem>();
            int total    = 0;
            int pageSize = 20;
            string connStr = _config.GetConnectionString("NominaDB");

            const string baseFrom = @"
                FROM titles t
                INNER JOIN employees e ON t.emp_no = e.emp_no
                WHERE (@search IS NULL
                       OR t.title       LIKE '%' + @search + '%'
                       OR e.first_name  LIKE '%' + @search + '%'
                       OR e.last_name   LIKE '%' + @search + '%'
                       OR e.ci          LIKE '%' + @search + '%')";

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
                    case "cargo":
                        orderByClause = $"ORDER BY t.title {(sortDirection.ToLower() == "desc" ? "DESC" : "ASC")}";
                        break;
                    case "empleado":
                        orderByClause = $"ORDER BY e.last_name {(sortDirection.ToLower() == "desc" ? "DESC" : "ASC")}, e.first_name {(sortDirection.ToLower() == "desc" ? "DESC" : "ASC")}";
                        break;
                    case "fecha":
                        orderByClause = $"ORDER BY t.from_date {(sortDirection.ToLower() == "desc" ? "DESC" : "ASC")}";
                        break;
                    default:
                        orderByClause = "ORDER BY t.title ASC";
                        break;
                }

                string dataSql = @"
                    SELECT t.emp_no,
                           e.first_name + ' ' + e.last_name AS full_name,
                           e.ci, t.title, t.from_date, t.to_date
                    " + baseFrom + @"
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
                            cargos.Add(new TitleListItem
                                {
                                    EmpNo     = Convert.ToInt32(reader.GetValue(0)),
                                    FullName  = reader.GetValue(1).ToString(),
                                    Ci        = reader.GetValue(2).ToString(),
                                    TitleName = reader.GetValue(3).ToString(),
                                    FromDate  = Convert.ToDateTime(reader.GetValue(4)),
                                    ToDate    = reader.IsDBNull(5) ? (DateTime?)null
                                                                   : Convert.ToDateTime(reader.GetValue(5))
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

            return View(cargos);
        }

        // GET: /Titles/Crear
        public IActionResult Crear()
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("rol") != "Admin")
            {
                TempData["Error"] = "Acceso denegado. Solo administradores.";
                return RedirectToAction("Index");
            }

            CargarEmpleados();
            return View();
        }

        // POST: /Titles/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(int empNo, string title, DateTime fromDate, DateTime? toDate)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("rol") != "Admin")
            {
                TempData["Error"] = "Acceso denegado. Solo administradores.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                ViewBag.Error = "El nombre del cargo es requerido.";
                CargarEmpleados();
                return View();
            }

            if (toDate.HasValue && toDate.Value < fromDate)
            {
                ViewBag.Error = "La fecha de fin no puede ser anterior a la fecha de inicio.";
                CargarEmpleados();
                return View();
            }

            string connStr = _config.GetConnectionString("NominaDB");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_register_title", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@p_emp_no",    empNo);
                    cmd.Parameters.AddWithValue("@p_title",     title);
                    cmd.Parameters.AddWithValue("@p_from_date", fromDate);
                    cmd.Parameters.AddWithValue("@p_to_date",   toDate.HasValue ? (object)toDate.Value : DBNull.Value);

                    SqlParameter pMsg = new SqlParameter("@r_message", System.Data.SqlDbType.VarChar, 200)
                    { Direction = System.Data.ParameterDirection.Output };
                    cmd.Parameters.Add(pMsg);

                    cmd.ExecuteNonQuery();

                    string msg = pMsg.Value?.ToString() ?? string.Empty;
                    if (msg.StartsWith("SUCCESS"))
                    {
                        RegistrarActividad("Titles", "CREATE", $"Cargo asignado: emp_no={empNo}, cargo={title}");
                        TempData["Exito"] = "Cargo asignado correctamente.";
                        return RedirectToAction("Index");
                    }

                    ViewBag.Error = msg.Replace("ERROR: ", "");
                    CargarEmpleados();
                    return View();
                }
            }
        }

        // GET: /Titles/Details/1013
        public IActionResult Details(int id)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            string connStr = _config.GetConnectionString("NominaDB");
            var historial  = new List<TitleListItem>();
            string fullName = string.Empty;
            string ci       = string.Empty;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
                    SELECT e.emp_no,
                           e.first_name + ' ' + e.last_name AS full_name,
                           e.ci, t.title, t.from_date, t.to_date
                    FROM titles t
                    INNER JOIN employees e ON t.emp_no = e.emp_no
                    WHERE t.emp_no = @empNo
                    ORDER BY t.from_date DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@empNo", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            fullName = reader.GetValue(1).ToString();
                            ci       = reader.GetValue(2).ToString();
                            historial.Add(new TitleListItem
                            {
                                EmpNo     = Convert.ToInt32(reader.GetValue(0)),
                                FullName  = fullName,
                                Ci        = ci,
                                TitleName = reader.GetValue(3).ToString(),
                                FromDate  = Convert.ToDateTime(reader.GetValue(4)),
                                ToDate    = reader.IsDBNull(5) ? (DateTime?)null
                                                               : Convert.ToDateTime(reader.GetValue(5))
                            });
                        }
                    }
                }
            }

            if (!historial.Any())
                return NotFound();

            ViewBag.EmpNo    = id;
            ViewBag.FullName = fullName;
            ViewBag.Ci       = ci;
            ViewBag.Usuario  = HttpContext.Session.GetString("usuario");

            return View(historial);
        }

        // GET: /Titles/Edit/1013
        public IActionResult Edit(int id)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("rol") != "Admin")
            {
                TempData["Error"] = "Acceso denegado. Solo administradores.";
                return RedirectToAction("Index");
            }

            TitleListItem model = null;
            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
                    SELECT TOP 1 t.emp_no,
                           e.first_name + ' ' + e.last_name AS full_name,
                           e.ci, t.title, t.from_date, t.to_date
                    FROM titles t
                    INNER JOIN employees e ON t.emp_no = e.emp_no
                    WHERE t.emp_no = @empNo
                    ORDER BY t.from_date DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@empNo", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model = new TitleListItem
                            {
                                EmpNo     = Convert.ToInt32(reader.GetValue(0)),
                                FullName  = reader.GetValue(1).ToString(),
                                Ci        = reader.GetValue(2).ToString(),
                                TitleName = reader.GetValue(3).ToString(),
                                FromDate  = Convert.ToDateTime(reader.GetValue(4)),
                                ToDate    = reader.IsDBNull(5) ? (DateTime?)null
                                                               : Convert.ToDateTime(reader.GetValue(5))
                            };
                        }
                    }
                }
            }

            if (model == null)
                return NotFound();

            ViewBag.Usuario = HttpContext.Session.GetString("usuario");
            return View(model);
        }

        // POST: /Titles/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int empNo, string originalTitle, DateTime originalFromDate,
                                  string newTitle, DateTime? toDate)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("rol") != "Admin")
            {
                TempData["Error"] = "Acceso denegado. Solo administradores.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(newTitle))
            {
                TempData["Error"] = "El nombre del cargo es requerido.";
                return RedirectToAction("Edit", new { id = empNo });
            }

            string connStr = _config.GetConnectionString("NominaDB");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
                    UPDATE titles
                    SET title   = @newTitle,
                        to_date = @toDate
                    WHERE emp_no    = @empNo
                      AND title     = @originalTitle
                      AND from_date = @fromDate";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@empNo",         empNo);
                    cmd.Parameters.AddWithValue("@originalTitle", originalTitle);
                    cmd.Parameters.AddWithValue("@fromDate",      originalFromDate);
                    cmd.Parameters.AddWithValue("@newTitle",      newTitle);
                    cmd.Parameters.AddWithValue("@toDate",        toDate.HasValue ? (object)toDate.Value : DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }

            TempData["Exito"] = "Cargo actualizado correctamente.";
            RegistrarActividad("Titles", "UPDATE", $"Cargo actualizado: emp_no={empNo}, nuevo cargo={newTitle}");
            return RedirectToAction("Details", new { id = empNo });
        }

        // ─── HELPERS ────────────────────────────────────────────────────

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

        private void CargarEmpleados()
        {
            var empleados = new List<EmployeeListItem>();
            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT emp_no, ci, first_name + ' ' + last_name AS full_name
                      FROM employees WHERE is_active = 1
                      ORDER BY last_name, first_name", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        empleados.Add(new EmployeeListItem
                        {
                            EmpNo    = Convert.ToInt32(reader.GetValue(0)),
                            Ci       = reader.GetValue(1).ToString(),
                            FullName = reader.GetValue(2).ToString()
                        });
                    }
                }
            }

            ViewBag.Empleados = empleados;
        }
    }
}
