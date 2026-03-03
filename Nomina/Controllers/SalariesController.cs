using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Nomina.Models;

namespace Nomina.Controllers
{
    public class SalariesController : Controller
    {
        private readonly IConfiguration _config;

        public SalariesController(IConfiguration config)
        {
            _config = config;
        }

        private bool VerificarSesion() =>
            HttpContext.Session.GetString("usuario") != null;

        // GET: /Salaries
        public IActionResult Index()
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            var salarios = new List<SalaryListItem>();
            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT s.id_salary, s.emp_no, " +
                    "e.first_name + ' ' + e.last_name AS full_name, " +
                    "s.salary, s.from_date, s.to_date " +
                    "FROM salaries s " +
                    "INNER JOIN employees e ON s.emp_no = e.emp_no " +
                    "WHERE s.to_date IS NULL " +
                    "ORDER BY e.last_name, e.first_name", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        salarios.Add(new SalaryListItem
                        {
                            IdSalario = Convert.ToInt32(reader.GetValue(0)),
                            EmpNo     = Convert.ToInt32(reader.GetValue(1)),
                            FullName  = reader.GetValue(2).ToString(),
                            Salary    = Convert.ToDecimal(reader.GetValue(3)),
                            FromDate  = Convert.ToDateTime(reader.GetValue(4)),
                            ToDate    = reader.IsDBNull(5) ? null : Convert.ToDateTime(reader.GetValue(5))
                        });
                    }
                }
            }

            return View(salarios);
        }

        // GET: /Salaries/Details/5
        public IActionResult Details(int id)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            SalaryListItem? salario = CargarSalario(id);
            if (salario == null) return NotFound();

            return View(salario);
        }

        // GET: /Salaries/Edit/5
        public IActionResult Edit(int id)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("rol") != "Admin")
            {
                TempData["Error"] = "Acceso denegado. Solo los administradores pueden editar.";
                return RedirectToAction("Index");
            }

            SalaryListItem? salario = CargarSalario(id);
            if (salario == null) return NotFound();

            return View("Editar", salario);
        }

        // POST: /Salaries/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id_salary, int emp_no, decimal salary, DateTime from_date, DateTime? to_date)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("rol") != "Admin")
            {
                TempData["Error"] = "Acceso denegado. Solo los administradores pueden editar.";
                return RedirectToAction("Index");
            }

            string userSession = HttpContext.Session.GetString("usuario");
            string connStr     = _config.GetConnectionString("NominaDB");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_update_salary", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_emp_no",       emp_no);
                    cmd.Parameters.AddWithValue("@p_salary",       Convert.ToInt64(salary));
                    cmd.Parameters.AddWithValue("@p_from_date",    from_date);
                    cmd.Parameters.AddWithValue("@p_user_session", userSession);

                    SqlParameter pMsg = new SqlParameter("@r_message", System.Data.SqlDbType.VarChar, 200)
                    { Direction = System.Data.ParameterDirection.Output };
                    cmd.Parameters.Add(pMsg);

                    SqlParameter pPrevSalary = new SqlParameter("@r_prev_salary", System.Data.SqlDbType.BigInt)
                    { Direction = System.Data.ParameterDirection.Output };
                    cmd.Parameters.Add(pPrevSalary);

                    cmd.ExecuteNonQuery();

                    string msg = pMsg.Value?.ToString() ?? string.Empty;
                    if (msg.StartsWith("SUCCESS"))
                    {
                        RegistrarActividad("Salaries", "UPDATE", $"Salario actualizado: emp_no={emp_no}, monto={salary}");
                        TempData["Exito"] = "Salario actualizado correctamente.";
                        return RedirectToAction("Index");
                    }

                    TempData["Error"] = msg.Replace("ERROR: ", "");
                    return RedirectToAction("Edit", new { id = id_salary });
                }
            }
        }

        // GET: /Salaries/Crear
        public IActionResult Crear()
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("rol") != "Admin")
            {
                TempData["Error"] = "Acceso denegado. Solo los administradores pueden registrar salarios.";
                return RedirectToAction("Index");
            }

            return View();
        }

        // POST: /Salaries/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(int emp_no, decimal salary, DateTime from_date, DateTime? to_date)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("rol") != "Admin")
            {
                TempData["Error"] = "Acceso denegado. Solo los administradores pueden registrar salarios.";
                return RedirectToAction("Index");
            }

            string userSession = HttpContext.Session.GetString("usuario");
            string connStr     = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_insert_salary", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_emp_no",       emp_no);
                    cmd.Parameters.AddWithValue("@p_salary",       salary);
                    cmd.Parameters.AddWithValue("@p_from_date",    from_date);
                    cmd.Parameters.AddWithValue("@p_to_date",      (object?)to_date ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_user_session", userSession);

                    SqlParameter pMsg = new SqlParameter("@r_message", System.Data.SqlDbType.VarChar, 200)
                    { Direction = System.Data.ParameterDirection.Output };
                    cmd.Parameters.Add(pMsg);

                    cmd.ExecuteNonQuery();

                    string msg = pMsg.Value?.ToString() ?? string.Empty;
                    if (msg.StartsWith("SUCCESS"))
                    {
                        RegistrarActividad("Salaries", "CREATE", $"Salario creado: emp_no={emp_no}, monto={salary}");
                        TempData["Exito"] = "Salario registrado correctamente.";
                        return RedirectToAction("Index");
                    }

                    ViewBag.Error = msg.Replace("ERROR: ", "");
                    return View();
                }
            }
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

        private SalaryListItem? CargarSalario(int id)
        {
            string connStr = _config.GetConnectionString("NominaDB");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT s.id_salary, s.emp_no, " +
                    "e.first_name + ' ' + e.last_name AS full_name, " +
                    "s.salary, s.from_date, s.to_date " +
                    "FROM salaries s " +
                    "INNER JOIN employees e ON s.emp_no = e.emp_no " +
                    "WHERE s.id_salary = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new SalaryListItem
                            {
                                IdSalario = Convert.ToInt32(reader.GetValue(0)),
                                EmpNo     = Convert.ToInt32(reader.GetValue(1)),
                                FullName  = reader.GetValue(2).ToString(),
                                Salary    = Convert.ToDecimal(reader.GetValue(3)),
                                FromDate  = Convert.ToDateTime(reader.GetValue(4)),
                                ToDate    = reader.IsDBNull(5) ? null : Convert.ToDateTime(reader.GetValue(5))
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}
