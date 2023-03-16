using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DAL.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ProductId { get; set; }

        [StringLength(100)]
        public string ProductName { get; set; }
        [StringLength(500)]
        public string ProductDescription { get; set; }
        [StringLength(50)]
        public string Color { get; set; }
        [Column(TypeName = "decimal(13,4)")]
        public decimal ListPrice { get; set; }
        [Column(TypeName = "decimal(13,4)")]
        public decimal StandardCost { get; set; }

        public Guid SubCategoryId { get; set; }
        public SubCategory SubCategory { get; set; }
    }
}
