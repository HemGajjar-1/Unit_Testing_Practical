using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical_30.Domain.Entities
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public decimal Salary { get; set; }
        public Department DepartmentId { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string EmailId { get; set; }
        public DateTime JoiningDate { get; set; } = DateTime.Now;
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}
