using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nomina.Models
{
    [Table("logauditori")]
    public class LogAuditory
    {
        [Key]
        [Column("id_log")]
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

        [ForeignKey("EmpNo")]
        public Employee Employee { get; set; }
    }
}