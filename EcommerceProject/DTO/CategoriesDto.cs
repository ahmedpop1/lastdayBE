using System.Collections.Generic;

namespace EcommerceProject.DTO
{
    public class CategoriesDto
    {
        public int ID { get; set; }
        public string CatName { get; set; }
        public virtual List<ProductDto>? Products { get; set; }
        = new List<ProductDto>();
    }

    public class ProductDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string image { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool Availability { get; set; }
        public float? discountPercentage { get; set; }
    }
}
