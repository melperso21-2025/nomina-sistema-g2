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
                            IdSalario = reader.GetInt32(0),
                            EmpNo     = reader.GetInt32(1),
                            FullName  = reader.GetString(2),
                            Salary = Convert.ToDecimal(reader.GetValue(3)),
                            FromDate  = reader.GetDateTime(4),
                            ToDate    = reader.IsDBNull(5) ? null : reader.GetDateTime(5)
                        });
                    }
                }
            }

            return View(salarios);
        }

        // GET: /Salarios/Crear
        public IActionResult Crear()
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            return View();
        }

        // POST: /Salarios/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(int emp_no, decimal salary, DateTime from_date, DateTime? to_date)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

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
                        TempData["Exito"] = "Salario registrado correctamente.";
                        return RedirectToAction("Index");
                    }

                    ViewBag.Error = msg.Replace("ERROR: ", "");
                    return View();
                }
            }
        }

        // GET: /Salaries/Editar/5
        public IActionResult Editar(int id)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            SalaryListItem? salario = null;
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
                            salario = new SalaryListItem
                            {
                                IdSalario = reader.GetInt32(0),
                                EmpNo     = reader.GetInt32(1),
                                FullName  = reader.GetString(2),
                                Salary    = reader.GetDecimal(3),
                                FromDate  = reader.GetDateTime(4),
                                ToDate    = reader.IsDBNull(5) ? null : reader.GetDateTime(5)
                            };
                        }
                    }
                }
            }

            if (salario == null) return NotFound();
            return View(salario);
        }

        // POST: /Salarios/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id_salary, decimal salary, DateTime from_date, DateTime? to_date)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            string connStr = _config.GetConnectionString("NominaDB");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_update_salary", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_id_salary", id_salary);
                    cmd.Parameters.AddWithValue("@p_salary",    salary);
                    cmd.Parameters.AddWithValue("@p_from_date", from_date);
                    cmd.Parameters.AddWithValue("@p_to_date",   (object?)to_date ?? DBNull.Value);

                    SqlParameter pMsg = new SqlParameter("@r_message", System.Data.SqlDbType.VarChar, 200)
                    { Direction = System.Data.ParameterDirection.Output };
                    cmd.Parameters.Add(pMsg);

                    cmd.ExecuteNonQuery();

                    string msg = pMsg.Value?.ToString() ?? string.Empty;
                    if (msg.StartsWith("SUCCESS"))
                    {
                        TempData["Exito"] = "Salario actualizado correctamente.";
                        return RedirectToAction("Index");
                    }

                    TempData["Error"] = msg.Replace("ERROR: ", "");
                    return RedirectToAction("Editar", new { id = id_salary });
                }
            }
        }
    }
}
