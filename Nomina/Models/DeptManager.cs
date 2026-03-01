using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nomina.Models
{
    [Table("dept_manager")]
    public class DeptManager
    {
        [Key]
        [Column("id_dept_manager")]
        public int IdDeptManager { get; set; }

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

        [ForeignKey("empNo")]
        public Employee Employee { get; set; }

        [ForeignKey("deptNo")]
        public Department Department { get; set; }
    }
}