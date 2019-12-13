using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiTest2.Entities;
using ApiTest2.Interfaces;
using ApiTest2.Models.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiTest2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequestModel model)
        {
            var createdUser = await _userService.RegisterAsync(model.Username, model.Password, model.Firstname, model.Lastname, model.Company);

            if (!createdUser.Success)
                return BadRequest(createdUser.Errors);

            return Ok(createdUser.Message);
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestModel model)
        {
            //var vpd = new ValidationProblemDetails { };

            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(s => s.Errors.Select(x => x.ErrorMessage)));

            var userToAuthenticate = await _userService.AuthenticateAsync(model.Username, model.Password);

            if (!userToAuthenticate.Success)
                return BadRequest(userToAuthenticate.Message);

            var successfulAuthResult = new Models.Transfer.UserLoginResponseModel
            {
                Token = userToAuthenticate.Token,
                RefreshToken = userToAuthenticate.RefreshToken
            };

            return Ok(successfulAuthResult);
        }


        [HttpGet]
        [Route("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestModel model)
        {
            var authResponse = await _userService.RefreshTokenAsync(model.Token, model.RefreshToken);

            if (!authResponse.Success)
                return BadRequest(authResponse.Message);

            var successfulAuthResult = new Models.Transfer.UserLoginResponseModel
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            };

            return Ok(successfulAuthResult);
        }
    }
}