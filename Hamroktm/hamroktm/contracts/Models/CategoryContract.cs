using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;

namespace contracts.Models
{
    public class CategoryContract
    {
        public CategoryData Category;
        public List<SubCategoryContract> SubCategories;

        public CategoryContract()
        {
            Category = new CategoryData();
            SubCategories=new List<SubCategoryContract>();
        }
    }

    public class CategoryData
    {
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
    }

    public class SubCategoryContract
    {
        public int SubCategoryId { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
