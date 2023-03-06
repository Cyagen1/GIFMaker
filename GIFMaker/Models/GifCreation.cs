using GIFMaker.Attributes;
using System.ComponentModel.DataAnnotations;

namespace GIFMaker.Models
{
    public class GifCreation
    {
        [Required(AllowEmptyStrings = false)]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Only alphanumerical characters allowed.")]
        public string Name { get; set; }

        [MaxLength(10, ErrorMessage = "Max 10 images allowed.")]
        [MinLength(2, ErrorMessage = "Pick at least two image files.")]
        [Required]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".bmp" }, ErrorMessage = "You must select correct image files.")]
        public IEnumerable<IFormFile> ImageFiles { get; set; }
    }
}
