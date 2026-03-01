using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nomina.Models
{
    [Table("titles")]
    public class Title
    {
        [Key]
        [Column("id_title")]
        public int IdTitulo { get; set; }

        [Required]
        [Column("emp_no")]
        public int EmpNo { get; set; }

        [Required]
        [StringLength(50)]
        [Column("title")]
        public string TitleName { get; set; }

        [Required]
        [Column("from_date")]
        public DateTime FromDate { get; set; }

        [Column("to_date")]
        public DateTime? ToDate { get; set; }

        [ForeignKey("empNo")]
        public Employee Employee { get; set; }
    }
}