using System.ComponentModel.DataAnnotations;

namespace kaufor.Models
{
    public class Salon
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Salon Name is required.")]
        [StringLength(100, ErrorMessage = "Salon Name can't be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Salon Type is required.")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Working Hours are required.")]
        public string WorkingHours { get; set; }

    }
}



