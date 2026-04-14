using Microsoft.EntityFrameworkCore;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Exceptions;
using tivitApi.Mappers;
using System.Security.Cryptography;
using tivitApi.Infra.SQS;
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
        private readonly IPasswordHasher _passwordHasher;
        private readonly SQSProducer _queue;
        private readonly ILogger<MatriculaService> _logger;

        public ProfessorService(AppDbContext context, IPasswordHasher passwordHasher, SQSProducer queue, ILogger<MatriculaService> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _queue = queue;
            _logger = logger;
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

                // Garante 6 d�gitos e m�nimo 100000
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

            return professores.Select(professor => professor.ToDTO()).ToList();
        }

        public async Task<List<ProfessorDTOResponse>> GetAllProfessoresAtivos()
        {
            var professores = await _context.Professores.Where(p => p.Status == "ATIVO").ToListAsync();

            return professores.Select(professor => professor.ToDTO()).ToList();
        }

        public async Task CriarProfessor(ProfessorDTORequest dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var senha = GerarSenha();
                var senhaHash = _passwordHasher.Hash(senha);

                var rm = await GerarRm();
                var professor = dto.ToEntity(senhaHash, rm);

                _context.Professores.Add(professor);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Envio de evento SQS fora da transação (operação externa)
                try
                {
                    await _queue.EnviarEventoAsync(new ProfessorStatusEvento
                    {
                        Rm = professor.Rm,
                        Nome = professor.Nome,
                        Email = professor.Email,
                        Status = "APROVADO",
                        SenhaGerada = senha,
                        Cpf = professor.Cpf
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao enviar evento para SQS");
                    // Não falha a operação se o envio do evento falhar
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }



    }
}