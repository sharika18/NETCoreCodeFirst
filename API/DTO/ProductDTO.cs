using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTO
{
    public class ProductDTO
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string Color { get; set; }
        public decimal ListPrice { get; set; }
        public decimal StandardCost { get; set; }
    }

    public class ProductWithSubCategoryDTO : ProductDTO
    {
        public SubCategoryDTO SubCategory { get; set; }
    }

    public class ProductCreateDTO : ProductDTO
    {
        public Guid SubCategoryId { get; set; }
    }
}
