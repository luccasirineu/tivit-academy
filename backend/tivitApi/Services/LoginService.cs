using tivitApi.Data;
using tivitApi.Models;
using tivitApi.DTOs;
using tivitApi.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace tivitApi.Services
{
    public interface ILoginService
    {
        Task<object> LoginAsync(LoginDTO request);
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

        public async Task<object> LoginAsync(LoginDTO loginDTO)
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

        // ─── Metodos privados por tipo ───────────────────────────────────────────

        private async Task<object> AutenticarAluno(LoginDTO loginDTO)
        {
            var aluno = await _context.Alunos
                .FirstOrDefaultAsync(a => a.Cpf == loginDTO.Cpf && a.Status == "ATIVO");

            ValidarCredenciais(aluno?.Senha, loginDTO.Senha);

            _logger.LogInformation($"Login de aluno realizado com sucesso: {loginDTO.Cpf}");
            return new { sucesso = true, tipo = "aluno", id = aluno!.Id };
        }

        private async Task<object> AutenticarProfessor(LoginDTO loginDTO)
        {
            var professor = await _context.Professores
                .FirstOrDefaultAsync(p => p.Cpf == loginDTO.Cpf && p.Status == "ATIVO");

            ValidarCredenciais(professor?.Senha, loginDTO.Senha);

            _logger.LogInformation($"Login de professor realizado com sucesso: {loginDTO.Cpf}");
            return new { sucesso = true, tipo = "professor", id = professor!.Id };
        }

        private async Task<object> AutenticarAdministrador(LoginDTO loginDTO)
        {
            var admin = await _context.Administradores
                .FirstOrDefaultAsync(a => a.Cpf == loginDTO.Cpf && a.Status == "ATIVO");

            ValidarCredenciais(admin?.Senha, loginDTO.Senha);

            _logger.LogInformation($"Login de administrador realizado com sucesso: {loginDTO.Cpf}");
            return new { sucesso = true, tipo = "administrador", id = admin!.Id };
        }

        // ─── Validacao centralizada ──────────────────────────────────────────────

        private void ValidarCredenciais(string? hashSalvo, string senhaDigitada)
        {
            bool usuarioExiste = hashSalvo != null;
            bool senhaValida = usuarioExiste && _passwordHasher.Verificar(senhaDigitada, hashSalvo!);

            if (!usuarioExiste || !senhaValida)
                throw new CredenciaisInvalidasException("Usuário ou senha inválidos");
        }
    }
}