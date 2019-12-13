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
        Task<AuthenticatonResult> AuthenticateAsync(string username, string password);

        Task<AuthenticatonResult> RegisterAsync(string username, string password, string firstname, string lastname, string company = null);

        Task<AuthenticatonResult> RefreshTokenAsync(string token, string refresh);
    }
}
