using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EcommerceProject.models
{
    public class CartItems
    {
        [ForeignKey("Cart")]
        public int CartId { get; set; }
        [JsonIgnore]

        public virtual Cart Cart { get; set; }

        [ForeignKey("product")]
        public int productID { get; set; }
        //[JsonIgnore]
        public Product product { get; set; }

        public int Quantity { get; set; }


    }
}
