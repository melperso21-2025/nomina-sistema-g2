using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Nomina.Data;
using Nomina.Models;
using Microsoft.EntityFrameworkCore;

namespace Nomina.Controllers
{
    public class SalariosController : Controller
    {
        private readonly IConfiguration _config;
        private readonly NominaContext  _context;

        public SalariosController(IConfiguration config, NominaContext context)
        {
            _config  = config;
            _context = context;
        }

        private bool VerificarSesion() =>
            HttpContext.Session.GetString("usuario") != null;

        // GET: /Salarios
        public async Task<IActionResult> Index()
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            var salarios = await _context.Salaries
                .OrderByDescending(s => s.FromDate)
                .Take(50)
                .ToListAsync();

            ViewBag.Usuario = HttpContext.Session.GetString("usuario");
            ViewBag.Rol     = HttpContext.Session.GetString("rol");
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
        public IActionResult Crear(int emp_no, long salary, DateTime from_date, DateTime? to_date)
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

        // GET: /Salarios/Editar/5
        public IActionResult Editar(int id)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            return View();
        }

        // POST: /Salarios/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id_salary, long salary, DateTime from_date, DateTime? to_date)
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

                    ViewBag.Error = msg.Replace("ERROR: ", "");
                    return View();
                }
            }
        }
    }
}
