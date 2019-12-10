using ApiTest2.Entities;
using ApiTest2.Interfaces;
using ApiTest2.Models.Configuration;
using ApiTest2.Models.Transfer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTest2.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly JwtSettings _jwtSettings;

        public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration, JwtSettings jwtSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _jwtSettings = jwtSettings;
        }

        public async Task<AuthenticatonResult> AuthenticateAsync(UserLoginRequestModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthenticatonResult> RegisterAsync(string username, string password, string firstname, string lastname, string company = null)
        {
            var existingUser = await _userManager.FindByEmailAsync(username);

            if (existingUser != null)
                return new AuthenticatonResult
                {
                    Success = false,
                    Message = "User already exists. Please sign in instead."
                };

            var newUser = new AppUser
            {
                Email = username,
                UserName = username,
                Firstname = firstname,
                Lastname = lastname,
                Company = company
            };

            var result = await _userManager.CreateAsync(newUser, password);

            if (!result.Succeeded)
                return new AuthenticatonResult
                {
                    Success = false,
                    Errors = result.Errors
                };

            return new AuthenticatonResult
            {
                Success = true,
                Message = $"An email has been sent to: {username}. Please verify your account"
            };
        }

        private string GenerateToken(AppUser user)
        {
            return "";
        }
    }
}
