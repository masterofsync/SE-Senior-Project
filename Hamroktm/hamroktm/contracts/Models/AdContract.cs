using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace contracts.Models
{
    public class AdContract
    {
        public int AdId { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [AllowHtml]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; }

        [Display(Name = "Updated On")]
        public DateTime UpdatedOn { get; set; }

        [Display(Name = "Views")]
        public int? Views { get; set; }

        [Required]
        [Display(Name = "Condition")]
        public string Condition { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Required]
        [Display(Name = "Start On")]
        public DateTime StartOn { get; set; }

        [Display(Name = "End On")]
        public DateTime EndOn { get; set; }

        [Required]
        [Display(Name = "Price")]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string Category { get; set; }

        [Required]
        [Display(Name = "SubCategory")]
        public string SubCategory { get; set; }

        public bool? Featured { get; set; }

        public List<string> Tags { get; set; }
        public List<string> Images { get; set; }

        public AdContract()
        {
            Tags = new List<string>();
            Images = new List<string>();
        }
    }
}
