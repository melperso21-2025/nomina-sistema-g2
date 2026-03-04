using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nomina.Models
{
    [Table("salary_audit_log")]
    public class LogAuditory
    {
        [Key]
        [Column("log_id")]
        public int IdLog { get; set; }

        [Column("emp_no")]
        public int EmpNo { get; set; }

        [Column("previus_salary")]
        public decimal? SalarioAnterior { get; set; }

        [Column("new_salary")]
        public decimal SalarioNuevo { get; set; }

        [Column("from_date")]
        public DateTime FechaCambio { get; set; }

        [StringLength(50)]
        [Column("user_session")]
        public string UserResponsable { get; set; }

        [Required]
        [Column("action_date")]
        public DateTime ActionDate { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey("empNo")]
        public Employee Employee { get; set; }
    }
}