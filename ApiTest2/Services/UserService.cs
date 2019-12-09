using ApiTest2.Entities;
using ApiTest2.Interfaces;
using ApiTest2.Models.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTest2.Services
{
    public class UserService : IUserService
    {
        public AppUser Authenticate(UserLoginRequestModel model)
        {
            throw new NotImplementedException();
        }

        public AppUser Create(UserRegisterRequestModel model)
        {
            throw new NotImplementedException();
        }
    }
}
