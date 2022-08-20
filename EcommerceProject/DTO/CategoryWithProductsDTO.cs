using System.Collections.Generic;

namespace EcommerceProject.DTO
{
    public class CategoryWithProductsDTO
    {
        public int ID  { get; set; }
        public string Name { get; set; }
        public virtual List<ProductDTo> products { get; set; }
            = new List<ProductDTo>();
    }
    public class ProductDTo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string image { get; set; }
        public decimal Price { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public bool Availability { get; set; }
        public float? discountPercentage { get; set; }
    }
}
