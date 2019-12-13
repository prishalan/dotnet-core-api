using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTest2.Models.Transfer
{
    public class RefreshTokenRequestModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
