using ApiTest2.Entities;
using ApiTest2.Models.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTest2.Interfaces
{
    public interface IUserService
    {
        AppUser Authenticate(UserLoginRequestModel model);

        AppUser Create(UserRegisterRequestModel model);
    }
}
