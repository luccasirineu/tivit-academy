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
        Task<ProfessorDTOResponse> GetProfessorById(int professorId);
        Task<List<ProfessorDTOResponse>> GetAllProfessores();
        Task<List<ProfessorDTOResponse>> GetAllProfessoresAtivos();

    }

    public class ProfessorService : IProfessorService
    {
        private readonly AppDbContext _context;

        public ProfessorService(AppDbContext context)
        {
            _context = context;
        }

        private ProfessorDTOResponse ConvertProfessorToProfessorDTOResponse(Professor professor)
        {
            return new ProfessorDTOResponse(
                professor.Id,
                professor.Nome,
                professor.Email,
                professor.Rm,
                professor.Cpf,
                professor.Status
                );
        }

        public async Task<int> GetQntdProfessoresAtivos()
        {
            return await _context.Professores.CountAsync();

        }

        public async Task<ProfessorDTOResponse> GetProfessorById(int professorId)
        {
            return await _context.Professores
                .Where(p => p.Id == professorId)
                .Select(p => new ProfessorDTOResponse
                {
                    Nome = p.Nome,
                    Email = p.Email,
                    Rm = p.Rm,
                    Cpf = p.Cpf,
                    Status = p.Status
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<ProfessorDTOResponse>> GetAllProfessores()
        {
            var professores = await _context.Professores.ToListAsync();

            // converter lista de Professores -> lista de ProfessorDTOResponse
            var resultado = professores
            .Select(professor => ConvertProfessorToProfessorDTOResponse(professor))
            .ToList();

            return resultado;
        }

        public async Task<List<ProfessorDTOResponse>> GetAllProfessoresAtivos()
        {
            var professores = await _context.Professores.Where(p => p.Status == "ATIVO").ToListAsync();

            // converter lista de Professores -> lista de ProfessorDTOResponse
            var resultado = professores
            .Select(professor => ConvertProfessorToProfessorDTOResponse(professor))
            .ToList();

            return resultado;
        }

    }
}