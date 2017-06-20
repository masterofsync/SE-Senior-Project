using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;

namespace contracts.Models
{
    public class UserContract
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Image { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime ?CreatedOn { get; set; }
        public List<string> Role { get; set; }
        public int FollowersCount { get; set; }
        public int Followingcount { get; set; }


        public UserContract()
        {
            Role = new List<string>();
        }

        //public bool HasRegistered { get; set; }

        //public string LoginProvider { get; set; }
    }
}
