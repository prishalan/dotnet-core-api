using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTest2.Models.Enums
{
    public enum ResponseFailureStatusCode
    {
        BadRequest = 400,
        Conflict = 409,
        Forbid = 403,
        NoContent = 204,
        NotFound = 404,
        Unauthorized = 401,
        UnprocessableEntity = 409,
        ValidationProblem = 400
    }
}
