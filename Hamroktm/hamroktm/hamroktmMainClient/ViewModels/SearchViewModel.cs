using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using contracts.Models;

namespace hamroktmMainClient.ViewModels
{
    public class SearchViewModel
    {
        public List<AdContract> adContract { get; set; }
        public string searchString { get; set; }

        public SearchViewModel()
        {
            adContract = new List<AdContract>();
            searchString = null;
        }
    }
}