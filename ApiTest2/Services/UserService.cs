using ApiTest2.Entities;
using ApiTest2.Interfaces;
using ApiTest2.Models.Configuration;
using ApiTest2.Models.Transfer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
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



        public async Task<AuthenticatonResult> AuthenticateAsync(string username, string password)
        {
            var user = await _userManager.FindByEmailAsync(username);

            if (user == null)
                return new AuthenticatonResult
                {
                    Success = false,
                    Message = "User does not exist. [[ REMOVE FOR PRODUCTION ]]"
                };

            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!userHasValidPassword)
                return new AuthenticatonResult
                {
                    Success = false,
                    Message = "User password failed. [[ REMOVE FOR PRODUCTION ]]"
                };

            if (!user.EmailConfirmed)
                return new AuthenticatonResult
                {
                    Success = false,
                    Message = "Your email has not yet been confirmed. [[ REMOVE FOR PRODUCTION ]]"
                };

            return new AuthenticatonResult
            {
                Success = true,
                Token = GenerateAuthenticationToken(user)
            };
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



        private string GenerateAuthenticationToken(AppUser user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.GivenName, user.Firstname),
                    new Claim(JwtRegisteredClaimNames.FamilyName, user.Lastname),
                    new Claim("id", user.Id),
                }),
                    Expires = DateTime.UtcNow.AddMinutes((double)_jwtSettings.ExpiryInMinutes),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
