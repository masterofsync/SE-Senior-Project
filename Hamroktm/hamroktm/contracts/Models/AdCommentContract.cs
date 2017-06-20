using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;

namespace contracts.Models
{
    public class AdCommentContract
    {
        
        public int CommentId { get; set; }
        [Required]
        public string Description { get; set; }
        
        public int AdId { get; set; }
        
        public DateTime CreatedOn { get; set; }
       
        public string CreatedBy { get; set; }

        public string UserImage { get; set; }

    }
}
