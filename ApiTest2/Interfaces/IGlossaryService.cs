using System.Collections.Generic;
using System.Threading.Tasks;
using ApiTest2.Entities;

namespace ApiTest2.Interfaces
{
    public interface IGlossaryService
    {
        Task<List<Glossary>> GetGlossaryTerms(string userId);
    }
}