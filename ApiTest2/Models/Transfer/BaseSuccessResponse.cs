using ApiTest2.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTest2.Models.Transfer
{
    public class BaseSuccessResponse<T>
    {
        public ResponseSuccessStatusCode Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
