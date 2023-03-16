using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTO
{
    public class SubCategoryDTO
    {
        public Guid SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
    }

    public class SubCategoryWithCategoryDTO : SubCategoryDTO
    {
        public CategoryDTO CategoryDTO { get; set; }
    }
}
