using tivitApi.Data;
using tivitApi.Models;
using tivitApi.DTOs;
using tivitApi.Infra.SQS;
using tivitApi.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;

namespace tivitApi.Services
{
    public interface ILoginService
    {
        Task<object> LoginAsync(LoginDTO request);
    }

    public class LoginService : ILoginService
    {
        private readonly AppDbContext _context;

        public LoginService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> LoginAsync(LoginDTO loginDTO)
        {
            if (loginDTO == null)
                return "Requisição inválida";

            string tipo = loginDTO.Tipo?.ToLower();

            switch (tipo)
            {
                case "aluno":
                    var aluno = await _context.Alunos
                        .FirstOrDefaultAsync(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha);

                    if (aluno == null)
                        return "Usuário ou senha inválidos";

                    return new { sucesso = true, tipo = "aluno", id = aluno.Id };

                case "professor":
                    var professor = await _context.Professores
                        .FirstOrDefaultAsync(p => p.Email == loginDTO.Email && p.Senha == loginDTO.Senha);

                    if (professor == null)
                        return "Usuário ou senha inválidos";

                    return new { sucesso = true, tipo = "professor", id = professor.Id };

                case "administrador":
                    var admin = await _context.Administradores
                        .FirstOrDefaultAsync(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha);

                    if (admin == null)
                        return "Usuário ou senha inválidos";

                    return new { sucesso = true, tipo = "administrador", id = admin.Id };

                default:
                    return "Tipo de login inválido";
            }
        }
    }
}