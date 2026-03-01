using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nomina.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id_user")]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(50)]
        [Column("username")]
        public string Username { get; set; }

        [Required]
        [StringLength(64)]
        [Column("password_hash")]
        public string PasswordHash { get; set; }  // SHA256 = 64 chars

        [Required]
        [StringLength(20)]
        [Column("role")]
        public string Rol { get; set; }  // "Admin" o "RRHH"

        [Column("emp_no")]
        public int? EmpNo { get; set; }  // nullable: puede ser un usuario sin empleado

        [ForeignKey("EmpNo")]
        public Employee Employee { get; set; }
    }
}