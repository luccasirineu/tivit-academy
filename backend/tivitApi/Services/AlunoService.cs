using Microsoft.EntityFrameworkCore;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Exceptions;
using tivitApi.Infra.SQS;
using System.Text;
using System.Security.Cryptography;

namespace tivitApi.Services
{
    public interface IAlunoService
    {
        Task<AlunoDTO> GetInfoAluno(int alunoId);
        Task<List<AlunoDTO>> GetAllAlunosByCurso(int cursoId);
        Task<List<AlunoDTO>> GetAllAlunosByTurmaId(int turmaId);
        Task<AlunoDTO> GetAlunoByMatriculaId(int matriculaId);
        Task<int> GetQntdAlunosAtivos();
        Task<List<AlunoDTO>> GetAllAlunos();
        Task UpdateTurmaAluno(int alunoId, int turmaId);
        Task ResetSenha(string cpf);
    }

    public class AlunoService : IAlunoService
    {
        private readonly AppDbContext _context;
        private readonly SQSProducer _queue;
        private readonly ILogger<AlunoService> _logger;
        private readonly IPasswordHasher _passwordHasher;

        public AlunoService(AppDbContext context, SQSProducer queue, ILogger<AlunoService> logger, IPasswordHasher passwordHasher)
        {
            _context = context;
            _queue = queue;
            _logger = logger;
            _passwordHasher = passwordHasher;

        }

        private AlunoDTO ConvertAlunoToAlunoDto(Aluno aluno)
        {
            var cursoNome = aluno.Matricula?.curso?.Nome ?? string.Empty;
            var turmaNome = string.Empty;

            if (aluno.TurmaId > 0)
            {
                turmaNome = _context.Turmas
                    .Where(t => t.Id == aluno.TurmaId)
                    .Select(t => t.Nome)
                    .FirstOrDefault() ?? string.Empty;
            }

            _logger.LogDebug("AlunoId: {AlunoId} | TurmaId: {TurmaId} | CursoNome: {CursoNome} | TurmaNome: {TurmaNome}", 
                aluno.Id, aluno.TurmaId, cursoNome, turmaNome);

            return new AlunoDTO(
                aluno.Nome,
                aluno.Email,
                aluno.Cpf,
                aluno.MatriculaId,
                aluno.Id,
                aluno.TurmaId,
                cursoNome,
                turmaNome
            );
        }


        private AlunoDTO ConvertAtributosToAlunoDTO(string Nome, string Email, string Cpf, int Id, int alunoId, int turmaId)
        {
            return new AlunoDTO(
                Nome,
                Email,
                Cpf,
                Id,
                alunoId,
                turmaId
                );
        }

        private string GerarSenha(int tamanho = 12)
        {
            const string caracteres =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%&*";

            // Gera bytes criptograficamente 
            byte[] bytes = RandomNumberGenerator.GetBytes(tamanho);

            var sb = new StringBuilder();

            foreach (byte b in bytes)
            {
                sb.Append(caracteres[b % caracteres.Length]);
            }

            return sb.ToString();
        }


        public async Task<List<AlunoDTO>> GetAllAlunosByCurso(int cursoId)
        {
            var alunosByCurso = await (
                from m in _context.Matriculas
                join a in _context.Alunos on m.Id equals a.MatriculaId
                where m.CursoId == cursoId && m.Status == "APROVADO"
                select new
                {
                    MatriculaId = m.Id,
                    AlunoId = a.Id,
                    TurmaId = a.TurmaId,
                    m.Nome,
                    m.Email,
                    m.Cpf
                }
            ).ToListAsync();

            if (!alunosByCurso.Any())
                throw new NotFoundException("Matricula", $"CursoId: {cursoId}");

            return alunosByCurso
                .Select(x => ConvertAtributosToAlunoDTO(
                    x.Nome,
                    x.Email,
                    x.Cpf,
                    x.MatriculaId,
                    x.AlunoId,
                    x.TurmaId
                ))
                .ToList();
        }

        public async Task<List<AlunoDTO>> GetAllAlunosByTurmaId(int turmaId)
        {
            var turmaExiste = await _context.Turmas.AnyAsync(t => t.Id == turmaId);
            if (!turmaExiste)
                throw new NotFoundException("Turma", turmaId);

            // Query otimizada com Include para carregar Matricula e Curso de uma vez
            var alunos = await _context.Alunos
                .Include(a => a.Matricula)
                    .ThenInclude(m => m.curso)
                .Where(a => a.TurmaId == turmaId)
                .OrderBy(a => a.Nome)
                .ToListAsync();

            return alunos.Select(aluno => ConvertAlunoToAlunoDto(aluno)).ToList();
        }

        public async Task<AlunoDTO> GetInfoAluno(int alunoId)
        {
            // Query otimizada com Include para carregar tudo de uma vez
            var aluno = await _context.Alunos
                .Include(a => a.Matricula)
                    .ThenInclude(m => m.curso)
                .Where(a => a.Id == alunoId)
                .FirstOrDefaultAsync();

            if (aluno == null)
                throw new NotFoundException("Aluno", alunoId);

            if (aluno.Matricula == null)
                throw new NotFoundException("Matricula", aluno.MatriculaId);

            if (aluno.Matricula.curso == null)
                throw new NotFoundException("Curso", aluno.Matricula.CursoId);

            return new AlunoDTO
            {
                Nome = aluno.Nome,
                Email = aluno.Email,
                Cpf = aluno.Cpf,
                MatriculaId = aluno.MatriculaId,
                CursoNome = aluno.Matricula.curso.Nome,
                TurmaId = aluno.TurmaId
            };
        }

        public async Task<AlunoDTO> GetAlunoByMatriculaId(int matriculaId)
        {
            var aluno = await _context.Alunos
                .Where(a => a.MatriculaId == matriculaId)
                .Select(a => new
                {
                    a.Id,
                    a.Nome,
                    a.Email,
                    a.Cpf,
                    a.MatriculaId,
                    a.TurmaId
                })
                .FirstOrDefaultAsync();

            if (aluno == null)
                throw new NotFoundException("Aluno", $"MatriculaId: {matriculaId}");

            return new AlunoDTO
            {
                Nome = aluno.Nome,
                Email = aluno.Email,
                Cpf = aluno.Cpf,
                MatriculaId = aluno.MatriculaId,
                AlunoId = aluno.Id,
                TurmaId = aluno.TurmaId
            };
        }

        public async Task<int> GetQntdAlunosAtivos()
        {
            return await _context.Alunos.CountAsync();

        }

        public async Task<List<AlunoDTO>> GetAllAlunos()
        {
            // Query otimizada com Include para carregar Matricula e Curso de uma vez
            var alunos = await _context.Alunos
                .Include(a => a.Matricula)
                    .ThenInclude(m => m.curso)
                .ToListAsync();

            return alunos.Select(aluno => ConvertAlunoToAlunoDto(aluno)).ToList();
        }

        public async Task UpdateTurmaAluno(int alunoId, int turmaId)
        {
            var aluno = await _context.Alunos
                .FirstOrDefaultAsync(a => a.Id == alunoId);

            if (aluno == null)
                throw new NotFoundException("Aluno", alunoId);

            var turmaExiste = await _context.Turmas
                .AnyAsync(t => t.Id == turmaId);

            if (!turmaExiste)
                throw new NotFoundException("Turma", turmaId);

            aluno.TurmaId = turmaId;
            await _context.SaveChangesAsync();
        }

        public async Task ResetSenha(string cpf)
        {
            _logger.LogInformation("Resetando senha do cpf: {Cpf}", cpf);

            if (string.IsNullOrWhiteSpace(cpf))
                throw new ValidationException("CPF inválido.");

            try
            {
                var aluno = await _context.Alunos.FirstOrDefaultAsync(a => a.Cpf == cpf);

                if (aluno == null)
                    throw new NotFoundException("Aluno", $"CPF: {cpf}");

                var senhaGerada = GerarSenha();
                var senhaHash = _passwordHasher.Hash(senhaGerada);
                aluno.Senha = senhaHash;
                await _context.SaveChangesAsync();

                try
                {
                    await _queue.EnviarEventoAsync(new MatriculaStatusEvento
                    {
                        MatriculaId = aluno.MatriculaId,
                        Nome = aluno.Nome,
                        Email = aluno.Email,
                        Status = "RESET_PASSWORD",
                        SenhaGerada = senhaGerada,
                        Cpf = aluno.Cpf
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao enviar evento para SQS");
                }
            }
            catch (Exception ex) when (ex is not NotFoundException && ex is not ValidationException)
            {
                _logger.LogError(ex, "Erro ao resetar senha"); 
                throw new BusinessException($"Erro ao resetar senha: {ex.Message}");
            }
        }
    }
}