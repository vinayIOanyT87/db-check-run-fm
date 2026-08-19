using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FMWebAPI.Models
{
    public class LoginRequestDTO
    {
        public string username { get; set; }
        public string password { get; set; }
        public string site { get; set; }
    }
}