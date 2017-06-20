using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using contracts.Models;

namespace hamroktmMainClient.ViewModels
{
    public class CommentsViewModel
    {
        public CommentsViewModel()
        {
            adCreator = "";
            AdComments=new List<AdCommentContract>();
        }
        public IEnumerable<AdCommentContract> AdComments { get; set; }
        public string adCreator { get; set; }

    }
}