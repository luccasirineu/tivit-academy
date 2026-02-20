using Microsoft.EntityFrameworkCore;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Exceptions;
using System.Security.Cryptography;

using System.Text.Json;
using System.Text;
using System.Security.Cryptography;


namespace tivitApi.Services
{
    public interface IProfessorService
    {
        
        Task<int> GetQntdProfessoresAtivos();
        Task<ProfessorDTOResponse> GetProfessorById(int professorId);
        Task<List<ProfessorDTOResponse>> GetAllProfessores();
        Task<List<ProfessorDTOResponse>> GetAllProfessoresAtivos();
        Task CriarProfessor(ProfessorDTORequest dto);

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

        private Professor ConvertProfessorRequestToProfessor(ProfessorDTORequest professorDTO, string senha, string rm)
        {
            var professor = new Professor
            {

                Nome = professorDTO.Nome,
                Email = professorDTO.Email,
                Cpf = professorDTO.Cpf,
                Rm = rm,
                Senha = senha,
                Status = professorDTO.Status
            };

            return professor;
        }

        private string GerarSenha(int tamanho = 12)
        {
            const string caracteres =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%&*";

            byte[] bytes = RandomNumberGenerator.GetBytes(tamanho);

            var sb = new StringBuilder();

            foreach (byte b in bytes)
            {
                sb.Append(caracteres[b % caracteres.Length]);
            }

            return sb.ToString();
        }

        public async Task<string> GerarRm()
        {
            const int minimo = 100000;
            const int maximo = 999999;

            const long a = 1664525;
            const long c = 1013904223;
            const long m = 2147483648; 

            long seed = DateTime.UtcNow.Ticks % m;
            int numeroGerado;
            bool existe;

            do
            {
                // LCG
                seed = (a * seed + c) % m;

                // Garante 6 dígitos e mínimo 100000
                numeroGerado = (int)(minimo + (seed % (maximo - minimo + 1)));

                string rmFormatado = $"RM{numeroGerado}";

                existe = await _context.Professores
                    .AnyAsync(p => p.Rm == rmFormatado);

            } while (existe);

            return $"RM{numeroGerado}";
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

        public async Task CriarProfessor(ProfessorDTORequest dto)
        {
            var senha = GerarSenha();
            var rm = await GerarRm();
            var professor = ConvertProfessorRequestToProfessor(dto, senha, rm);

            _context.Professores.Add(professor);
            await _context.SaveChangesAsync();

        }



    }
}