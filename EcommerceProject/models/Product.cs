using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EcommerceProject.models

{
    public class Product
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string ImageName { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

        [NotMapped]
        public string ImageSrc { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Quantity { get; set; }

        [ForeignKey("Brand")]
        public int? BrandID { get; set; }
        //[JsonIgnore]
        public Brand Brand { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        //[JsonIgnore]
        public Category Category { get; set; }
        [Required]
        public bool Availability { get; set; }
        public float? discountPercentage { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderDetials> OrderDetials { get; set; }
        = new HashSet<OrderDetials>();
    

    }
}
