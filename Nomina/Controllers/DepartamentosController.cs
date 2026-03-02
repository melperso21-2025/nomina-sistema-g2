using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Nomina.Models;

namespace Nomina.Controllers
{
    public class DepartamentosController : Controller
    {
        private readonly IConfiguration _config;

        public DepartamentosController(IConfiguration config)
        {
            _config = config;
        }

        private bool VerificarSesion() =>
            HttpContext.Session.GetString("usuario") != null;

        // GET: /Departamentos
        public IActionResult Index()
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            ViewBag.Usuario = HttpContext.Session.GetString("usuario");
            ViewBag.Rol     = HttpContext.Session.GetString("rol");
            return View();
        }

        // GET: /Departamentos/Crear
        public IActionResult Crear()
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            return View();
        }

        // POST: /Departamentos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(string dept_no, string dept_name)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            string connStr = _config.GetConnectionString("NominaDB");
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
                        TempData["Exito"] = "Departamento creado correctamente.";
                        return RedirectToAction("Index");
                    }

                    ViewBag.Error = msg.Replace("ERROR: ", "");
                    return View();
                }
            }
        }

        // GET: /Departamentos/Editar/5
        public IActionResult Editar(int id)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            return View();
        }

        // POST: /Departamentos/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(string dept_no, string dept_name)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            string connStr = _config.GetConnectionString("NominaDB");
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
                        TempData["Exito"] = "Departamento actualizado correctamente.";
                        return RedirectToAction("Index");
                    }

                    ViewBag.Error = msg.Replace("ERROR: ", "");
                    return View();
                }
            }
        }
    }
}
