using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EcommerceProject.models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public DateTime date { get; set; }
        [ForeignKey("user")]
        public string username { get; set; }
        [JsonIgnore]
        public user user { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderDetials> OrderDetials { get; set; }
         = new HashSet<OrderDetials>();
    }
}
