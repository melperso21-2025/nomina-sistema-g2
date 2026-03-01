using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nomina.Models
{
    [Table("departments")]
    public class Department
    {
        [Key]
        [StringLength(4)]
        [Column("dept_no")]
        public string DeptNo { get; set; }

        [Required]
        [StringLength(40)]
        [Column("dept_name")]
        public string DeptName { get; set; }

        [Column("active")]
        public bool Activo { get; set; } = true;
    }
}