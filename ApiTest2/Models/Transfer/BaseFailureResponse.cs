using ApiTest2.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTest2.Models.Transfer
{
    public class BaseFailureResponse<T>
    {
        public bool Success { get; } = false;
        public ResponseFailureStatusCode Status { get; set; }
        public string Message { get; set; }
        public IAsyncEnumerable<string> Errors { get; set; }
        public Exception Exception { get; set; } = null;
    }
}
