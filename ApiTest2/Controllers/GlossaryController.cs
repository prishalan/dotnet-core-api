using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiTest2.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiTest2.Extensions;

namespace ApiTest2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GlossaryController : ControllerBase
    {
        private readonly IGlossaryService _glossaryService;

        public GlossaryController(IGlossaryService glossaryService)
        {
            _glossaryService = glossaryService;
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var userId = HttpContext.GetUserId();
            var glossary = await _glossaryService.GetGlossaryTerms(userId);
            return Ok(glossary);
        }

        [HttpGet("test")]
        [Authorize]
        public async Task<IActionResult> Test()
        {
            return Ok(new { message = "This is a test!" });
        }
    }
}