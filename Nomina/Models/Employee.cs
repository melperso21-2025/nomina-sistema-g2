using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nomina.Models
{
    [Table("employees")]
    public class Employee
    {
        [Key]
        [Column("emp_no")]
        public int EmpNo { get; set; }

        [Required]
        [Column("birth_date")]
        public DateTime BirthDate { get; set; }


        [Required]
        [Column("ci")]
        [StringLength(50)]
        public string Ci { get; set; }

        [Required]
        [StringLength(14)]
        [Column("first_name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(16)]
        [Column("last_name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(1)]
        [Column("gender")]
        public string Gender { get; set; }

        [Required]
        [Column("hire_date")]
        public DateTime HireDate { get; set; }

        // Column remains 'is_active' in DB, property renamed to IsActive
        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [StringLength(100)]
        [Column("email")]
        public string Email { get; set; }
    }
}