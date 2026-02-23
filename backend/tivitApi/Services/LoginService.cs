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

            ValidarCredenciais(alunos.FirstOrDefault()?.Senha, loginDTO.Senha);

            _logger.LogInformation($"Login de aluno realizado com sucesso: {loginDTO.Cpf} | Cursos: {alunos.Count}");

            return new LoginDTOResponse
            {
                Tipo = "aluno",
                Cpf = loginDTO.Cpf,
                CursosIds = alunos.Select(a => a.Id).ToList()
            };
        }

        private async Task<LoginDTOResponse> AutenticarProfessor(LoginDTO loginDTO)
        {
            var professores = await _context.Professores
                .Where(p => p.Cpf == loginDTO.Cpf && p.Status == "ATIVO")
                .ToListAsync();

            ValidarCredenciais(professores.FirstOrDefault()?.Senha, loginDTO.Senha);

            _logger.LogInformation($"Login de professor realizado com sucesso: {loginDTO.Cpf} | Cursos: {professores.Count}");

            return new LoginDTOResponse
            {
                Tipo = "professor",
                Cpf = loginDTO.Cpf,
                CursosIds = professores.Select(p => p.Id).ToList()
            };
        }

        private async Task<LoginDTOResponse> AutenticarAdministrador(LoginDTO loginDTO)
        {
            var admins = await _context.Administradores
                .Where(a => a.Cpf == loginDTO.Cpf && a.Status == "ATIVO")
                .ToListAsync();

            ValidarCredenciais(admins.FirstOrDefault()?.Senha, loginDTO.Senha);

            _logger.LogInformation($"Login de administrador realizado com sucesso: {loginDTO.Cpf} | Registros: {admins.Count}");

            return new LoginDTOResponse
            {
                Tipo = "administrador",
                Cpf = loginDTO.Cpf,
                CursosIds = admins.Select(a => a.Id).ToList()
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