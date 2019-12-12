using ApiTest2.Data;
using ApiTest2.Entities;
using ApiTest2.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTest2.Services
{
    public class GlossaryService : IGlossaryService
    {
        private readonly AppDbContext _context;

        public GlossaryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Glossary>> GetGlossaryTerms(string userId)
        {
            try
            {
                return await _context.Glossary.Where(x => x.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
