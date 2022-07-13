using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalesOrder2021.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int? Department { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
    }
}