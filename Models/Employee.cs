using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kaufor.Models
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

        // Navigation property for Appointments
        public List<Appointment> Appointments { get; set; }

        // Calculated property for total earnings
        //[NotMapped] // Ensures this property is not stored in the database
        public decimal TotalEarnings => Appointments?.Sum(a => a.Service.Price) ?? 0;
    }
}
