using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Nomina.Models;

namespace Nomina.Controllers
{
    public class ActivityLogController : Controller
    {
        private readonly IConfiguration _config;

        public ActivityLogController(IConfiguration config)
        {
            _config = config;
        }

        private bool VerificarSesion() =>
            HttpContext.Session.GetString("usuario") != null;

        // GET: /ActivityLog
        public IActionResult Index(string module = null, int page = 1)
        {
            if (!VerificarSesion())
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("rol") != "Admin")
            {
                TempData["Error"] = "Acceso denegado. Solo administradores.";
                return RedirectToAction("Index", "Dashboard");
            }

            var registros  = new List<ActivityLogItem>();
            int total      = 0;
            int pageSize   = 50;
            string connStr = _config.GetConnectionString("NominaDB");

            const string baseFrom = @"
                FROM activity_log
                WHERE (@module IS NULL OR module = @module)";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                using (SqlCommand countCmd = new SqlCommand("SELECT COUNT(*) " + baseFrom, conn))
                {
                    countCmd.Parameters.AddWithValue("@module",
                        string.IsNullOrWhiteSpace(module) ? (object)DBNull.Value : module);
                    total = Convert.ToInt32(countCmd.ExecuteScalar());
                }

                string dataSql = @"
                    SELECT log_id, action_date, user_session, module, action, description
                    " + baseFrom + @"
                    ORDER BY action_date DESC
                    OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                using (SqlCommand cmd = new SqlCommand(dataSql, conn))
                {
                    cmd.Parameters.AddWithValue("@module",
                        string.IsNullOrWhiteSpace(module) ? (object)DBNull.Value : module);
                    cmd.Parameters.AddWithValue("@offset",   (page - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            registros.Add(new ActivityLogItem
                            {
                                LogId       = Convert.ToInt32(reader.GetValue(0)),
                                ActionDate  = reader.GetDateTime(1),
                                UserSession = reader.GetValue(2).ToString(),
                                Module      = reader.GetValue(3).ToString(),
                                Action      = reader.GetValue(4).ToString(),
                                Description = reader.IsDBNull(5) ? string.Empty : reader.GetValue(5).ToString()
                            });
                        }
                    }
                }
            }

            ViewBag.Total        = total;
            ViewBag.Page         = page;
            ViewBag.PageSize     = pageSize;
            ViewBag.ModuleFiltro = module;
            ViewBag.Usuario      = HttpContext.Session.GetString("usuario");
            ViewBag.Rol          = HttpContext.Session.GetString("rol");

            return View(registros);
        }
    }
}
