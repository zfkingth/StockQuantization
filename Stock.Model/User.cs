using System;
using System.Collections.Generic;

namespace Stock.Model
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string RoleName { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}