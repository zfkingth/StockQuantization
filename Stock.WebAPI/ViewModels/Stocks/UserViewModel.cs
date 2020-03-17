using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stock.WebAPI.ViewModels
{
    public class UserViewModel
    {
        public class User 
        {
        
            public string Id { get; set; }
            public string Username { get; set; }
            public string RoleName { get; set; }
            public DateTime ExpiredDate { get; set; }
            public string Email { get; set; }

        }

    }
}
