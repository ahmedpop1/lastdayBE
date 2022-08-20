using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcommerceProject.models
{
    public class Category
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string CatName { get; set; }
        [JsonIgnore]
        public virtual List<Product>? Products { get; set; }
    }
}
