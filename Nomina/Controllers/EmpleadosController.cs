using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using Nomina.Models;

namespace Nomina.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly IConfiguration _config;

        public EmpleadosController(IConfiguration config)
        {
            _config = config;
        }

        private bool VerificarSesion()
        {
            return HttpContext.Session.GetString("usuario") != null;
        }

        // GET: /Empleados
        public IActionResult Index(string filterName = null, string filterCi = null, int page = 1)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            var empleados = new List<EmployeeListItem>();
            int total = 0;
            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_list_employees", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@p_filter_name", string.IsNullOrEmpty(filterName) ? (object)DBNull.Value : filterName);
                    cmd.Parameters.AddWithValue("@p_filter_ci",   string.IsNullOrEmpty(filterCi)   ? (object)DBNull.Value : filterCi);
                    cmd.Parameters.AddWithValue("@p_dept_no",     DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_page",        page);
                    cmd.Parameters.AddWithValue("@p_page_size",   20);

                    SqlParameter pTotal = new SqlParameter("@r_total", System.Data.SqlDbType.Int)
                    { Direction = System.Data.ParameterDirection.Output };
                    cmd.Parameters.Add(pTotal);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            empleados.Add(new EmployeeListItem
                            {
                                EmpNo    = reader.GetInt32(0),
                                Ci       = reader.GetString(1),
                                FullName = reader.GetString(2),
                                Email    = reader.GetString(3),
                                HireDate = reader.GetDateTime(4),
                                Gender   = reader.GetString(5),
                                DeptName = reader.IsDBNull(6) ? "Sin departamento" : reader.GetString(6)
                            });
                        }
                    }

                    total = (int)pTotal.Value;
                }
            }

            ViewBag.Total      = total;
            ViewBag.Page       = page;
            ViewBag.FilterName = filterName;
            ViewBag.FilterCi   = filterCi;
            ViewBag.Usuario    = HttpContext.Session.GetString("usuario");
            ViewBag.Rol        = HttpContext.Session.GetString("rol");

            return View(empleados);
        }

        // GET: /Empleados/Detalle/5
        public IActionResult Detalle(int id)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            EmployeeDetail detalle = null;
            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_get_employee_detail", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_emp_no", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            detalle = new EmployeeDetail
                            {
                                EmpNo     = reader.GetInt32(0),
                                Ci        = reader.GetString(1),
                                FullName  = reader.GetString(2),
                                FirstName = reader.GetString(3),
                                LastName  = reader.GetString(4),
                                BirthDate = reader.GetDateTime(5),
                                Gender    = reader.GetString(6),
                                HireDate  = reader.GetDateTime(7),
                                Email     = reader.GetString(8),
                                IsActive  = reader.GetBoolean(9),
                                DeptName  = reader.IsDBNull(10) ? "Sin departamento" : reader.GetString(10),
                                DeptNo    = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11),
                                Title     = reader.IsDBNull(12) ? "Sin cargo" : reader.GetString(12),
                                Salary    = reader.IsDBNull(13) ? 0 : reader.GetInt64(13)
                            };
                        }
                    }
                }
            }

            if (detalle == null)
                return NotFound();

            ViewBag.Usuario = HttpContext.Session.GetString("usuario");
            ViewBag.Rol     = HttpContext.Session.GetString("rol");
            return View(detalle);
        }

        // GET: /Empleados/Crear
        public IActionResult Crear()
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            CargarDepartamentos();
            ViewBag.Usuario = HttpContext.Session.GetString("usuario");
            return View();
        }

        // POST: /Empleados/Crear
        [HttpPost]
        public IActionResult Crear(CreateEmployeeViewModel model)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                CargarDepartamentos();
                return View(model);
            }

            byte[] passwordHash = SHA256.HashData(Encoding.UTF8.GetBytes(model.Password));
            string userSession  = HttpContext.Session.GetString("usuario");
            string connStr      = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_insert_full_employee", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@p_emp_no",        model.EmpNo);
                    cmd.Parameters.AddWithValue("@p_ci",            model.Ci);
                    cmd.Parameters.AddWithValue("@p_first_name",    model.FirstName);
                    cmd.Parameters.AddWithValue("@p_last_name",     model.LastName);
                    cmd.Parameters.AddWithValue("@p_birth_date",    model.BirthDate);
                    cmd.Parameters.AddWithValue("@p_gender",        model.Gender);
                    cmd.Parameters.AddWithValue("@p_hire_date",     model.HireDate);
                    cmd.Parameters.AddWithValue("@p_email",         model.Email);
                    cmd.Parameters.AddWithValue("@p_password_hash", passwordHash);
                    cmd.Parameters.AddWithValue("@p_role",          model.Role);
                    cmd.Parameters.AddWithValue("@p_dept_no",       model.DeptNo);
                    cmd.Parameters.AddWithValue("@p_salary",        model.Salary);
                    cmd.Parameters.AddWithValue("@p_title",         model.Title);
                    cmd.Parameters.AddWithValue("@p_user_session",  userSession);

                    SqlParameter pMessage = new SqlParameter("@r_message", System.Data.SqlDbType.VarChar, 200)
                    { Direction = System.Data.ParameterDirection.Output };
                    cmd.Parameters.Add(pMessage);

                    cmd.ExecuteNonQuery();

                    string message = pMessage.Value.ToString();
                    if (message.StartsWith("SUCCESS"))
                    {
                        TempData["Exito"] = "Empleado registrado correctamente.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Error = message.Replace("ERROR: ", "");
                        CargarDepartamentos();
                        return View(model);
                    }
                }
            }
        }

        // GET: /Empleados/Editar/5
        public IActionResult Editar(int id)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            EditEmployeeViewModel model = null;
            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_get_employee_detail", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_emp_no", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model = new EditEmployeeViewModel
                            {
                                EmpNo     = reader.GetInt32(0),
                                FirstName = reader.GetString(3),
                                LastName  = reader.GetString(4),
                                BirthDate = reader.GetDateTime(5),
                                Gender    = reader.GetString(6),
                                Email     = reader.GetString(8)
                            };
                        }
                    }
                }
            }

            if (model == null) return NotFound();

            ViewBag.Usuario = HttpContext.Session.GetString("usuario");
            return View(model);
        }

        // POST: /Empleados/Editar
        [HttpPost]
        public IActionResult Editar(EditEmployeeViewModel model)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_update_employee", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@p_emp_no",     model.EmpNo);
                    cmd.Parameters.AddWithValue("@p_first_name", model.FirstName);
                    cmd.Parameters.AddWithValue("@p_last_name",  model.LastName);
                    cmd.Parameters.AddWithValue("@p_birth_date", model.BirthDate);
                    cmd.Parameters.AddWithValue("@p_gender",     model.Gender);
                    cmd.Parameters.AddWithValue("@p_email",      model.Email);

                    SqlParameter pMessage = new SqlParameter("@r_message", System.Data.SqlDbType.VarChar, 200)
                    { Direction = System.Data.ParameterDirection.Output };
                    cmd.Parameters.Add(pMessage);

                    cmd.ExecuteNonQuery();

                    string message = pMessage.Value.ToString();
                    if (message.StartsWith("SUCCESS"))
                    {
                        TempData["Exito"] = "Empleado actualizado correctamente.";
                        return RedirectToAction("Detalle", new { id = model.EmpNo });
                    }
                    else
                    {
                        ViewBag.Error = message.Replace("ERROR: ", "");
                        return View(model);
                    }
                }
            }
        }

        // POST: /Empleados/Desactivar/5
        [HttpPost]
        public IActionResult Desactivar(int id)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_deactivate_employee", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_emp_no", id);

                    SqlParameter pMessage = new SqlParameter("@r_message", System.Data.SqlDbType.VarChar, 200)
                    { Direction = System.Data.ParameterDirection.Output };
                    cmd.Parameters.Add(pMessage);

                    cmd.ExecuteNonQuery();

                    string message = pMessage.Value.ToString();
                    if (message.StartsWith("SUCCESS"))
                        TempData["Exito"] = "Empleado desactivado correctamente.";
                    else
                        TempData["Error"] = message.Replace("ERROR: ", "");
                }
            }

            return RedirectToAction("Index");
        }

        // ─── SALARIOS ───────────────────────────────────────────

        // POST: /Empleados/ActualizarSalario
        [HttpPost]
        public IActionResult ActualizarSalario(int empNo, long salary, DateTime fromDate)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            string userSession = HttpContext.Session.GetString("usuario");
            string connStr     = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_update_salary", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@p_emp_no",       empNo);
                    cmd.Parameters.AddWithValue("@p_salary",       salary);
                    cmd.Parameters.AddWithValue("@p_from_date",    fromDate);
                    cmd.Parameters.AddWithValue("@p_user_session", userSession);

                    SqlParameter pMessage    = new SqlParameter("@r_message",     System.Data.SqlDbType.VarChar, 200) { Direction = System.Data.ParameterDirection.Output };
                    SqlParameter pPrevSalary = new SqlParameter("@r_prev_salary", System.Data.SqlDbType.BigInt)       { Direction = System.Data.ParameterDirection.Output };

                    cmd.Parameters.Add(pMessage);
                    cmd.Parameters.Add(pPrevSalary);

                    cmd.ExecuteNonQuery();

                    string message = pMessage.Value.ToString();
                    if (message.StartsWith("SUCCESS"))
                        TempData["Exito"] = "Salario actualizado correctamente.";
                    else
                        TempData["Error"] = message.Replace("ERROR: ", "");
                }
            }

            return RedirectToAction("Detalle", new { id = empNo });
        }

        // ─── ASIGNACION DE DEPARTAMENTO ─────────────────────────

        // POST: /Empleados/AsignarDepartamento
        [HttpPost]
        public IActionResult AsignarDepartamento(int empNo, int deptNo, DateTime fromDate, DateTime? toDate)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_assign_department", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@p_emp_no",    empNo);
                    cmd.Parameters.AddWithValue("@p_dept_no",   deptNo);
                    cmd.Parameters.AddWithValue("@p_from_date", fromDate);
                    cmd.Parameters.AddWithValue("@p_to_date",   toDate.HasValue ? (object)toDate.Value : DBNull.Value);

                    SqlParameter pMessage = new SqlParameter("@r_message", System.Data.SqlDbType.VarChar, 200)
                    { Direction = System.Data.ParameterDirection.Output };
                    cmd.Parameters.Add(pMessage);

                    cmd.ExecuteNonQuery();

                    string message = pMessage.Value.ToString();
                    if (message.StartsWith("SUCCESS"))
                        TempData["Exito"] = "Departamento asignado correctamente.";
                    else
                        TempData["Error"] = message.Replace("ERROR: ", "");
                }
            }

            return RedirectToAction("Detalle", new { id = empNo });
        }

        // ─── REGISTRO DE TITULO/CARGO ────────────────────────────

        // POST: /Empleados/RegistrarTitulo
        [HttpPost]
        public IActionResult RegistrarTitulo(int empNo, string title, DateTime fromDate, DateTime? toDate)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

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

                    SqlParameter pMessage = new SqlParameter("@r_message", System.Data.SqlDbType.VarChar, 200)
                    { Direction = System.Data.ParameterDirection.Output };
                    cmd.Parameters.Add(pMessage);

                    cmd.ExecuteNonQuery();

                    string message = pMessage.Value.ToString();
                    if (message.StartsWith("SUCCESS"))
                        TempData["Exito"] = "Cargo registrado correctamente.";
                    else
                        TempData["Error"] = message.Replace("ERROR: ", "");
                }
            }

            return RedirectToAction("Detalle", new { id = empNo });
        }

        // ─── ASIGNACION DE MANAGER ───────────────────────────────

        // POST: /Empleados/AsignarManager
        [HttpPost]
        public IActionResult AsignarManager(int empNo, int deptNo, DateTime fromDate, DateTime? toDate)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_assign_manager", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@p_emp_no",    empNo);
                    cmd.Parameters.AddWithValue("@p_dept_no",   deptNo);
                    cmd.Parameters.AddWithValue("@p_from_date", fromDate);
                    cmd.Parameters.AddWithValue("@p_to_date",   toDate.HasValue ? (object)toDate.Value : DBNull.Value);

                    SqlParameter pMessage = new SqlParameter("@r_message", System.Data.SqlDbType.VarChar, 200)
                    { Direction = System.Data.ParameterDirection.Output };
                    cmd.Parameters.Add(pMessage);

                    cmd.ExecuteNonQuery();

                    string message = pMessage.Value.ToString();
                    if (message.StartsWith("SUCCESS"))
                        TempData["Exito"] = "Manager asignado correctamente.";
                    else
                        TempData["Error"] = message.Replace("ERROR: ", "");
                }
            }

            return RedirectToAction("Detalle", new { id = empNo });
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
                            DeptNo   = reader.GetInt32(0),
                            DeptName = reader.GetString(1)
                        });
                    }
                }
            }

            ViewBag.Departamentos = departamentos;
        }
    }
}
