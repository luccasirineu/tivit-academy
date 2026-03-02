using tivitApi.Data;
using tivitApi.Models;
using tivitApi.DTOs;
using tivitApi.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace tivitApi.Services
{
    public interface ILoginService
    {
        Task<LoginDTOResponse> LoginAsync(LoginDTO request);
    }

    public class LoginService : ILoginService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LoginService> _logger;
        private readonly IPasswordHasher _passwordHasher;

        public LoginService(AppDbContext context, ILogger<LoginService> logger, IPasswordHasher passwordHasher)
        {
            _context = context;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginDTOResponse> LoginAsync(LoginDTO loginDTO)
        {
            if (loginDTO == null)
                throw new RequisicaoInvalidaException("Requisição inválida");

            _logger.LogInformation($"Tentativa de login: {loginDTO.Cpf} | Tipo: {loginDTO.Tipo}");

            string tipo = loginDTO.Tipo?.ToLower();

            return tipo switch
            {
                "aluno" => await AutenticarAluno(loginDTO),
                "professor" => await AutenticarProfessor(loginDTO),
                "administrador" => await AutenticarAdministrador(loginDTO),
                _ => throw new RequisicaoInvalidaException("Tipo de login inválido")
            };
        }

        // ─── Métodos privados por tipo ───────────────────────────────────────────

        private async Task<LoginDTOResponse> AutenticarAluno(LoginDTO loginDTO)
        {
            var alunos = await _context.Alunos
                .Where(a => a.Cpf == loginDTO.Cpf && a.Status == "ATIVO")
                .ToListAsync();

            var alunosIds = alunos.Select(a => a.MatriculaId).ToList();

            var cursosIds = await _context.Matriculas
                .Where(m => alunosIds.Contains(m.Id))
                .Select(m => m.CursoId)
                .ToListAsync();

            ValidarCredenciais(alunos.FirstOrDefault()?.Senha, loginDTO.Senha);

            return new LoginDTOResponse
            {
                Id = alunos.FirstOrDefault()!.Id,
                Nome = alunos.FirstOrDefault()!.Nome,
                Tipo = "aluno",
                Cpf = loginDTO.Cpf,
                CursosIds = cursosIds,
                TurmaId = alunos.FirstOrDefault()!.TurmaId
            };
        }

        private async Task<LoginDTOResponse> AutenticarProfessor(LoginDTO loginDTO)
        {
            var professor = await _context.Professores
                .FirstOrDefaultAsync(p => p.Cpf == loginDTO.Cpf && p.Status == "ATIVO");

            ValidarCredenciais(professor?.Senha, loginDTO.Senha);

            return new LoginDTOResponse
            {
                Id = professor!.Id,
                Nome = professor.Nome,
                Tipo = "professor",
                Cpf = loginDTO.Cpf,
                CursosIds = new List<int> { professor.Id }
            };
        }

        private async Task<LoginDTOResponse> AutenticarAdministrador(LoginDTO loginDTO)
        {
            var admin = await _context.Administradores
                .FirstOrDefaultAsync(a => a.Cpf == loginDTO.Cpf && a.Status == "ATIVO");

            ValidarCredenciais(admin?.Senha, loginDTO.Senha);

            return new LoginDTOResponse
            {
                Id = admin!.Id,
                Nome = admin.Nome,
                Tipo = "administrador",
                Cpf = loginDTO.Cpf,
                CursosIds = new List<int> { admin.Id }
            };
        }

        // ─── Validação centralizada ──────────────────────────────────────────────

        private void ValidarCredenciais(string? hashSalvo, string senhaDigitada)
        {
            bool usuarioExiste = hashSalvo != null;
            bool senhaValida = usuarioExiste && _passwordHasher.Verificar(senhaDigitada, hashSalvo!);

            if (!usuarioExiste || !senhaValida)
                throw new CredenciaisInvalidasException("Usuário ou senha inválidos");
        }
    }
}