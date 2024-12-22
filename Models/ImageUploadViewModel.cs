using System.ComponentModel.DataAnnotations;

namespace kaufor.Models
{
    public class ImageUploadViewModel
    {
        [Required]
        [Display(Name = "Upload Image")]
        public IFormFile Image { get; set; }
    }
}
