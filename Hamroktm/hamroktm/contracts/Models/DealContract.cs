using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;

namespace contracts.Models
{
    public class DealContract
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public int DealAmount { get; set; }

        public string Message { get; set; }

        public int AdId { get; set; }

    }
}
