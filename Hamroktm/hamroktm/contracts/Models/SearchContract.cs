using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace contracts.Models
{
    public class SearchContract
    {
        public List<AdContract> adContract { get; set; }
        public string searchString { get; set; }
        public int TotalResult { get; set; }
        public int CurrentPage { get; set; }

        public SearchContract()
        {
            adContract = new List<AdContract>();
            searchString = null;
        }
    }
}
