using System.ComponentModel.DataAnnotations;

namespace Berber.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Employee Name is required.")]
        [StringLength(50, ErrorMessage = "Employee Name can't be longer than 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Specialization is required.")]
        [StringLength(50, ErrorMessage = "Specialization can't be longer than 50 characters.")]
        public string Specialization { get; set; }

        [Required(ErrorMessage = "Working Hours are required.")]
        public string WorkingHours { get; set; }

    }
}


    
