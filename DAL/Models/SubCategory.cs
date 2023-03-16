using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DAL.Models
{
    public class SubCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SubCategoryId { get; set; }

        public Guid CategoryId { get; set; }
        public Category Category { get; set; }

        [StringLength(100)]
        public string SubCategoryName { get; set; }

    }
}
