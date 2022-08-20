using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcommerceProject.models
{
    public class user
    {
        [Key]
        public string email  { get; set; }
        [Required]
        public string fullname { get; set; }
       
        [Required]

        public string phonenumber { get; set; }
        [Required]

        public byte[] passwordHash { get; set; }
        [Required]

        public byte[] passwordSalt { get; set; }
        [Required]

        public string address { get; set; }
        public byte[]? image { get; set; }
        [Required]
        public string type { get; set; }
        public string token { get; set; }
        public bool? isloggedin { get; set; }
        [JsonIgnore]
        public Cart cart { get; set; }
        [JsonIgnore]
        public virtual ICollection<Order> OrderDetials { get; set; }
         = new HashSet<Order>();
    }
}
