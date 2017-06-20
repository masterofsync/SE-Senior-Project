using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using contracts.Models;

namespace hamroktmMainClient.ViewModels
{
    public class PostAdViewModel
    {
        public AdContract Ad { get; set; }
        public CategoryContract Category { get; set; }

        public PostAdViewModel()
        {
            Ad=new AdContract();
            Category=new CategoryContract();
        }

    }
}