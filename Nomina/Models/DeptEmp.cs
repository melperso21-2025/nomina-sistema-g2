using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nomina.Models
{
    [Table("dept_emp")]
    public class DeptEmp
    {
        [Key]
        [Column("id_dept_emp")]
        public int IdDeptEmp { get; set; }

        [Required]
        [Column("emp_no")]
        public int EmpNo { get; set; }

        [Required]
        [StringLength(4)]
        [Column("dept_no")]
        public string DeptNo { get; set; }

        [Required]
        [Column("from_date")]
        public DateTime FromDate { get; set; }

        [Column("to_date")]
        public DateTime? ToDate { get; set; }

        [ForeignKey("EmpNo")]
        public Employee Employee { get; set; }

        [ForeignKey("DeptNo")]
        public Department Department { get; set; }
    }
}