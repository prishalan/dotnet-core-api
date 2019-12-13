using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTest2.Models.Configuration
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public int ExpiryInMinutes { get; set; }
        public TimeSpan TokenLifetime { get; set; }
    }
}
