using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kaufor.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Service Name is required.")]
        [StringLength(100, ErrorMessage = "Service Name can't be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 1000.00, ErrorMessage = "Price must be between $0.01 and $1000.")]
        [Column(TypeName = "decimal(18, 2)")] // Specify precision and scale
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Duration in minutes is required.")]
        [Range(1, 120, ErrorMessage = "Duration must be between 1 and 120 minutes.")]
        public int DurationInMinutes { get; set; }
    }
}
