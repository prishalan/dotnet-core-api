using ApiTest2.Data;
using ApiTest2.Entities;
using ApiTest2.Interfaces;
using ApiTest2.Models.Configuration;
using ApiTest2.Models.Transfer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly AppDbContext _context;

        public UserService(
            UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager, 
            IConfiguration configuration, 
            JwtSettings jwtSettings,
            TokenValidationParameters tokenValidationParameters,
            AppDbContext context
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _context = context;
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


        public async Task<AuthenticatonResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);
            
            if (validatedToken == null)
            {
                return new AuthenticatonResult
                {
                    Success = false,
                    Errors = new List<IdentityError>()
                    {
                        new IdentityError { Code = "TokenInvalid", Description = "The token provided is invalid." }
                    }
                };
            }

            //check if token has expired ... this check can be omitted
            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Value == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(expiryDateUnix).Subtract(_jwtSettings.TokenLifetime);
            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticatonResult
                {
                    Success = false,
                    Errors = new List<IdentityError>()
                    {
                        new IdentityError { Code = "TokenNotExpired", Description = "The token provided has not expired as yet. [[ REMOVE FOR PRODUCTION ]]" }
                    }
                };
            }


            // Get JwtIDfrom the Claim
            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            // validate that this token Id is the same as the refresh token 
            var storedRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);
            if (storedRefreshToken == null)
            {
                return new AuthenticatonResult
                {
                    Success = false,
                    Errors = new List<IdentityError>()
                    {
                        new IdentityError { Code = "RefreshTokenInvalid", Description = "The refresh token does not exist. [[ REMOVE FOR PRODUCTION ]]" }
                    }
                };
            }


            // check if the refresh token has not expired
            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticatonResult
                {
                    Success = false,
                    Errors = new List<IdentityError>()
                    {
                        new IdentityError { Code = "RefreshTokenInvalid", Description = "The refresh token has expired. [[ REMOVE FOR PRODUCTION ]]" }
                    }
                };
            }


            // check if the stored refresh token is invalid
            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticatonResult
                {
                    Success = false,
                    Errors = new List<IdentityError>()
                    {
                        new IdentityError { Code = "RefreshTokenInvalid", Description = "The refresh token has been invalidated. [[ REMOVE FOR PRODUCTION ]]" }
                    }
                };
            }


            // Lastly, check if the refresh token has been used
            if (storedRefreshToken.Used)
            {
                return new AuthenticatonResult
                {
                    Success = false,
                    Errors = new List<IdentityError>()
                    {
                        new IdentityError { Code = "RefreshTokenInvalid", Description = "The refresh token has already been used. [[ REMOVE FOR PRODUCTION ]]" }
                    }
                };
            }


            // 
            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticatonResult
                {
                    Success = false,
                    Errors = new List<IdentityError>()
                    {
                        new IdentityError { Code = "RefreshTokenInvalid", Description = "The refresh token does not match the JWT. [[ REMOVE FOR PRODUCTION ]]" }
                    }
                };
            }


            // else, all good
            storedRefreshToken.Used = true;
            _context.RefreshTokens.Update(storedRefreshToken);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);

            return await GenerateAuthenticationToken2(user);

            return new AuthenticatonResult
            {
                Success = true,
                Token = "",
                RefreshToken = ""
            };
        }



        private async Task<AuthenticatonResult> GenerateAuthenticationToken2(AppUser user)
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
                    Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return new AuthenticatonResult
                {
                    Success = true,
                    Token = tokenHandler.WriteToken(token),
                    RefreshToken = "" // TODO!!!!!!
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                    Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
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


        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (
                (validatedToken is JwtSecurityToken jwtSecurityToken)
                &&
                (jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            );
        }
    }
}
