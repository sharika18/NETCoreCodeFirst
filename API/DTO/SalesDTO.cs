using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTO
{
    public class SalesDTO
    {
        public Guid SalesId { get; set; }

        public DateTime OrderDate { get; set; }
        public int OrderQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SalesAmount { get; set; }
        public string SalesStatus { get; set; }
    }

    public class SalesWithDependencyDTO : SalesDTO
    {
        public CustomerDTO Customer { get; set; }
        public ProductDTO Product { get; set; }
        public TerritoriesDTO Territories { get; set; }
    }

    public class SalesCreateDTO
    {
        public Guid SalesId { get; set; }

        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
        public Guid TerritoriesID { get; set; }
        public int OrderQuantity { get; set; }
    }

    
    
}
