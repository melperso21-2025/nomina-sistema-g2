// =============================================
// Models/ViewModels.cs
// Todos los ViewModels del sistema de nomina
// =============================================

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nomina.Models
{
    // ─── LOGIN ───────────────────────────────────────────────────

    public class LoginViewModel
    {
        [Required(ErrorMessage = "El usuario es requerido")]
        public string Usuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Clave { get; set; } = string.Empty;
    }

    // ─── EMPLEADOS ───────────────────────────────────────────────

    public class EmployeeListItem
    {
        public int EmpNo { get; set; } = 0;
        public string Ci { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime HireDate { get; set; } = DateTime.MinValue;
        public string Gender { get; set; } = string.Empty;
        public string DeptName { get; set; } = string.Empty;
    }

    public class EmployeeDetail
    {
        public int EmpNo { get; set; } = 0;
        public string Ci { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; } = DateTime.MinValue;
        public string Gender { get; set; } = string.Empty;
        public DateTime HireDate { get; set; } = DateTime.MinValue;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
        public string DeptName { get; set; } = string.Empty;
        public int? DeptNo { get; set; }
        public string Title { get; set; } = string.Empty;
        public long Salary { get; set; } = 0;
    }

    public class CreateEmployeeViewModel
    {
        [Required(ErrorMessage = "El número de empleado es requerido")]
        public int EmpNo { get; set; } = 0;

        [Required(ErrorMessage = "La cédula es requerida")]
        public string Ci { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        public DateTime BirthDate { get; set; } = DateTime.MinValue;

        [Required(ErrorMessage = "El género es requerido")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de contratación es requerida")]
        public DateTime HireDate { get; set; } = DateTime.MinValue;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email no válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es requerido")]
        public string Role { get; set; } = string.Empty;

        [Required(ErrorMessage = "El departamento es requerido")]
        public string DeptNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El salario es requerido")]
        [Range(1, long.MaxValue, ErrorMessage = "El salario debe ser mayor a 0")]
        public long Salary { get; set; } = 0;

        [Required(ErrorMessage = "El cargo es requerido")]
        public string Title { get; set; } = string.Empty;

        public List<SelectListItem> Departamentos { get; set; } = new();
    }

    public class EditEmployeeViewModel
    {
        public int EmpNo { get; set; } = 0;

        public string Ci { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        public DateTime BirthDate { get; set; } = DateTime.MinValue;

        [Required(ErrorMessage = "El género es requerido")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email no válido")]
        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
    }

    // ─── DEPARTAMENTOS ───────────────────────────────────────────

    public class DepartmentItem
    {
        public string DeptNo { get; set; } = string.Empty;
        public string DeptName { get; set; } = string.Empty;
    }

    // ─── DASHBOARD ───────────────────────────────────────────────

    public class DashboardViewModel
    {
        public int TotalEmpleados { get; set; } = 0;
        public int TotalDepartamentos { get; set; } = 0;
        public long SalarioPromedio { get; set; } = 0;
        public List<SalaryChangeItem> UltimosCambios { get; set; } = new();
    }

    public class SalaryChangeItem
    {
        public DateTime ActionDate { get; set; } = DateTime.MinValue;
        public string FullName { get; set; } = string.Empty;
        public long PreviousSalary { get; set; } = 0;
        public long NewSalary { get; set; } = 0;
        public string UserSession { get; set; } = string.Empty;
    }

    // ─── SALARIOS ────────────────────────────────────────────────

    public class SalarioItem
    {
        public long Salary { get; set; } = 0;
        public string FromDate { get; set; } = string.Empty;
        public string ToDate { get; set; } = string.Empty;
    }

    public class SalarioViewModel
    {
        public int EmpNo { get; set; } = 0;
        public string FullName { get; set; } = string.Empty;
        public string Ci { get; set; } = string.Empty;
        public long SalarioVigente { get; set; } = 0;
        public string SalarioDesde { get; set; } = string.Empty;
        public List<SalarioItem> Historial { get; set; } = new();
    }

    // ─── AUDITORÍA ───────────────────────────────────────────────

    public class AuditoriaItem
    {
        public DateTime ActionDate { get; set; } = DateTime.MinValue;
        public string UserSession { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public long PreviousSalary { get; set; } = 0;
        public long NewSalary { get; set; } = 0;
    }

    public class AuditoriaViewModel
    {
        public List<AuditoriaItem> Registros { get; set; } = new();
        public string FechaInicio { get; set; } = string.Empty;
        public string FechaFin { get; set; } = string.Empty;
        public string CiEmpleado { get; set; } = string.Empty;
    }

    // ─── REPORTES ────────────────────────────────────────────────

    public class PayrollReportItem
    {
<<<<<<< HEAD
        public string   DeptName    { get; set; }
        public int      EmpNo       { get; set; }
        public string   FullName    { get; set; }
        public string   Ci          { get; set; }
        public string   Title       { get; set; }
        public decimal  Salary      { get; set; }
        public DateTime SalarySince { get; set; }
=======
        public string DeptName { get; set; } = string.Empty;
        public int EmpNo { get; set; } = 0;
        public string FullName { get; set; } = string.Empty;
        public string Ci { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public long Salary { get; set; } = 0;
        public DateTime SalarySince { get; set; } = DateTime.MinValue;
>>>>>>> origin/develop
    }

    public class SalaryChangesReportItem
    {
<<<<<<< HEAD
        public int      LogId          { get; set; }
        public DateTime ActionDate     { get; set; }
        public string   UserSession    { get; set; }
        public string   FullName       { get; set; }
        public string   Ci             { get; set; }
        public decimal  PreviousSalary { get; set; }
        public decimal  NewSalary      { get; set; }
        public DateTime FromDate       { get; set; }
=======
        public int LogId { get; set; } = 0;
        public DateTime ActionDate { get; set; } = DateTime.MinValue;
        public string UserSession { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Ci { get; set; } = string.Empty;
        public long PreviousSalary { get; set; } = 0;
        public long NewSalary { get; set; } = 0;
        public DateTime FromDate { get; set; } = DateTime.MinValue;
    }

    public class ReporteViewModel
    {
        public string TipoReporte { get; set; } = string.Empty;
        public string DeptNo { get; set; } = string.Empty;
        public string FechaDesde { get; set; } = string.Empty;
        public string FechaHasta { get; set; } = string.Empty;
        public List<ReporteItem> Resultados { get; set; } = new();
        public List<SelectListItem> Departamentos { get; set; } = new();
    }

    public class ReporteItem
    {
        public string DeptName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Ci { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public long Salary { get; set; } = 0;
        public string UserSession { get; set; } = string.Empty;
        public DateTime ActionDate { get; set; } = DateTime.MinValue;
>>>>>>> origin/develop
    }

    // ─── SALARIES ────────────────────────────────────────────────

    public class SalaryListItem
    {
        public int       IdSalario { get; set; }
        public int       EmpNo     { get; set; }
        public string    FullName  { get; set; }
        public decimal   Salary    { get; set; }
        public DateTime  FromDate  { get; set; }
        public DateTime? ToDate    { get; set; }
    }

    // ─── CARGOS (TITLES) ────────────────────────────────────────

    public class TitleListItem
    {
        public int       EmpNo     { get; set; }
        public string    FullName  { get; set; }
        public string    Ci        { get; set; }
        public string    TitleName { get; set; }
        public DateTime  FromDate  { get; set; }
        public DateTime? ToDate    { get; set; }
    }

    // ─── REPORTES — ESTRUCTURA ORGANIZACIONAL ───────────────────

    public class OrgChartItem
    {
        public string DeptName      { get; set; }
        public string ManagerName   { get; set; }
        public int    EmployeeCount { get; set; }
    }

    // ─── LOG DE ACTIVIDAD ────────────────────────────────────────

    public class ActivityLogItem
    {
        public int      LogId       { get; set; }
        public DateTime ActionDate  { get; set; }
        public string   UserSession { get; set; }
        public string   Module      { get; set; }
        public string   Action      { get; set; }
        public string   Description { get; set; }
    }
}
