using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DAL.Models
{
    public class Sales
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SalesId { get; set; }

        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }


        public Guid TerritoriesId { get; set; }
        public Territories Territories { get; set; }
    
        public DateTime OrderDate { get; set; }
        public int OrderQuantity { get; set; }
        [Column(TypeName = "decimal(13,4)")]
        public decimal UnitPrice { get; set; }
        [Column(TypeName = "decimal(13,4)")]
        public decimal SalesAmount { get; set; }

        public string SalesStatus { get; set; }

        public Sales()
        {
            OrderDate = DateTime.Now;
        }

        

    }
}
