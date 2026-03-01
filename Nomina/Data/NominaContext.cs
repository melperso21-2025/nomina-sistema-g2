using Microsoft.EntityFrameworkCore;
using Nomina.Models;

namespace Nomina.Data
{
    public class NominaContext : DbContext
    {
        public NominaContext(DbContextOptions<NominaContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<DeptEmp> DeptEmps { get; set; }
        public DbSet<DeptManager> DeptManagers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LogAuditory> LogAuditoria { get; set; }
    }
}