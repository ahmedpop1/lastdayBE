using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcommerceProject.models
{
    public class Brand
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string BName { get; set; }
        [JsonIgnore]
        public virtual List<Product>? Products { get; set; }

    }
}
