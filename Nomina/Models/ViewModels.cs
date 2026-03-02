// =============================================
// Models/ViewModels.cs
// Todos los ViewModels del sistema de nomina
// =============================================

using System.ComponentModel.DataAnnotations;

namespace Nomina.Models
{
    // ─── EMPLEADOS ───────────────────────────────────────────────

    public class EmployeeListItem
    {
        public int      EmpNo    { get; set; }
        public string   Ci       { get; set; }
        public string   FullName { get; set; }
        public string   Email    { get; set; }
        public DateTime HireDate { get; set; }
        public string   Gender   { get; set; }
        public string   DeptName { get; set; }
    }

    public class EmployeeDetail
    {
        public int      EmpNo     { get; set; }
        public string   Ci        { get; set; }
        public string   FullName  { get; set; }
        public string   FirstName { get; set; }
        public string   LastName  { get; set; }
        public DateTime BirthDate { get; set; }
        public string   Gender    { get; set; }
        public DateTime HireDate  { get; set; }
        public string   Email     { get; set; }
        public bool     IsActive  { get; set; }
        public string   DeptName  { get; set; }
        public int?     DeptNo    { get; set; }
        public string   Title     { get; set; }
        public long     Salary    { get; set; }
    }

    public class CreateEmployeeViewModel
    {
        [Required(ErrorMessage = "El número de empleado es requerido")]
        public int EmpNo { get; set; }

        [Required(ErrorMessage = "La cédula es requerida")]
        public string Ci { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "El género es requerido")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "La fecha de contratación es requerida")]
        public DateTime HireDate { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email no válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El rol es requerido")]
        public string Role { get; set; }

        [Required(ErrorMessage = "El departamento es requerido")]
        public string DeptNo { get; set; }

        [Required(ErrorMessage = "El salario es requerido")]
        [Range(1, long.MaxValue, ErrorMessage = "El salario debe ser mayor a 0")]
        public long Salary { get; set; }

        [Required(ErrorMessage = "El cargo es requerido")]
        public string Title { get; set; }
    }

    public class EditEmployeeViewModel
    {
        public int EmpNo { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "El género es requerido")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email no válido")]
        public string Email { get; set; }
    }

    // ─── DEPARTAMENTOS ───────────────────────────────────────────

    public class DepartmentItem
    {
        public string DeptNo   { get; set; }
        public string DeptName { get; set; }
    }

    // ─── DASHBOARD ───────────────────────────────────────────────

    public class DashboardViewModel
    {
        public int                   TotalEmpleados    { get; set; }
        public int                   TotalDepartamentos { get; set; }
        public long                  SalarioPromedio   { get; set; }
        public List<SalaryChangeItem> UltimosCambios   { get; set; } = new();
    }

    public class SalaryChangeItem
    {
        public DateTime ActionDate     { get; set; }
        public string   FullName       { get; set; }
        public long     PreviousSalary { get; set; }
        public long     NewSalary      { get; set; }
        public string   UserSession    { get; set; }
    }

    // ─── REPORTES ────────────────────────────────────────────────

    public class PayrollReportItem
    {
        public string   DeptName    { get; set; }
        public int      EmpNo       { get; set; }
        public string   FullName    { get; set; }
        public string   Ci          { get; set; }
        public string   Title       { get; set; }
        public decimal  Salary      { get; set; }
        public DateTime SalarySince { get; set; }
    }

    public class SalaryChangesReportItem
    {
        public int      LogId          { get; set; }
        public DateTime ActionDate     { get; set; }
        public string   UserSession    { get; set; }
        public string   FullName       { get; set; }
        public string   Ci             { get; set; }
        public decimal  PreviousSalary { get; set; }
        public decimal  NewSalary      { get; set; }
        public DateTime FromDate       { get; set; }
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
}
