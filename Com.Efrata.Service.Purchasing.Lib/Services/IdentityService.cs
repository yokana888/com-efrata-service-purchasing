using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.Services
{
    public class IdentityService
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public int TimezoneOffset { get; set; } = 7;
    }
}
