using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nomina.Models
{
    [Table("salaries")]
    public class Salary
    {
        [Key]
        [Column("id_salary")]
        public int IdSalario { get; set; }

        [Required]
        [Column("emp_no")]
        public int EmpNo { get; set; }

        [Required]
        [Column("salary")]
        public decimal SalaryAmount { get; set; }

        [Required]
        [Column("from_date")]
        public DateTime FromDate { get; set; }

        [Column("to_date")]
        public DateTime? ToDate { get; set; }

        // Navegación
        [ForeignKey("EmpNo")]
        public Employee Employee { get; set; }
    }
}