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


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok($"value: {id}");
        }
}
}