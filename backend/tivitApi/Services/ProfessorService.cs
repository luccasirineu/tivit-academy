using Microsoft.EntityFrameworkCore;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Exceptions;

namespace tivitApi.Services
{
    public interface IProfessorService
    {
        
        Task<int> GetQntdProfessoresAtivos();

    }

    public class ProfessorService : IProfessorService
    {
        private readonly AppDbContext _context;

        public ProfessorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetQntdProfessoresAtivos()
        {
            return await _context.Professores.CountAsync();

        }

    }
}