using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace contracts.Models
{
    public class FollowContract
    {
        public UserContract UserData { get; set; }
        public bool IsFollowing { get; set; }

        public FollowContract()
        {
            UserData = new UserContract();
            IsFollowing = false;
        }
    }
}
