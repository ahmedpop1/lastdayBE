using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace EcommerceProject.models
{
    public class ApplicationUser:IdentityUser<int>
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Max Length for Name 50 ")]
        [MinLength(5, ErrorMessage = "Max Length for Name 5 ")]
        [DataType(DataType.Text)]

        public string Name { get; set; }
        [Required]
        public string Address { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string ImageName { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

        [NotMapped]
        public string ImageSrc { get; set; }
    }
}
