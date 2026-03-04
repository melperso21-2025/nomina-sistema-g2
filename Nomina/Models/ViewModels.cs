// =============================================
// Models/ViewModels.cs
// Todos los ViewModels del sistema de nomina
// =============================================

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nomina.Models
{
    // ─── VALIDADORES PERSONALIZADOS ─────────────────────────────

    /// <summary>
    /// Validador que hace un campo requerido solo si otra propiedad tiene un valor específico
    /// </summary>
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _conditionalPropertyName;
        private readonly object _conditionalPropertyValue;

        public RequiredIfAttribute(string conditionalPropertyName, object conditionalPropertyValue)
        {
            _conditionalPropertyName = conditionalPropertyName;
            _conditionalPropertyValue = conditionalPropertyValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_conditionalPropertyName);
            if (property == null)
                return ValidationResult.Success;

            var conditionalValue = property.GetValue(validationContext.ObjectInstance);

            // Si la condición se cumple y el valor actual es nulo o vacío, es inválido
            if (Equals(conditionalValue, _conditionalPropertyValue))
            {
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                    return new ValidationResult(ErrorMessage ?? $"El campo {validationContext.DisplayName} es requerido.");
            }

            return ValidationResult.Success;
        }
    }

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
        public bool IsActive { get; set; } = true;
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

        // Sistema de acceso - Opcional
        public bool RequiresSystemAccess { get; set; } = false;

        // Password y Role son opcionales - se validan en el controlador si RequiresSystemAccess = true
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        [Required(ErrorMessage = "El departamento es requerido")]
        public int DeptNo { get; set; } = 0;

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
        public bool IsActive { get; set; } = true;
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
        public bool      IsActive  { get; set; } = true;
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
        public bool      IsActive  { get; set; } = true;
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
