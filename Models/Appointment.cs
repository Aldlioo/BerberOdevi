using System.ComponentModel.DataAnnotations;

namespace kaufor.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Appointment Date and Time is required.")]
        public DateTime DateTime { get; set; }

        [Required(ErrorMessage = "Salon is required.")]
        public int SalonId { get; set; }
        public Salon Salon { get; set; }

        [Required(ErrorMessage = "Employee is required.")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [Required(ErrorMessage = "Service is required.")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [Required(ErrorMessage = "Customer Name is required.")]
        [StringLength(100, ErrorMessage = "Customer Name can't be longer than 100 characters.")]
        public string CustomerName { get; set; }
        //h
    }
}
