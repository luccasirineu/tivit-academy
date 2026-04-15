using Microsoft.EntityFrameworkCore;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Exceptions;
using tivitApi.Enums;

namespace tivitApi.Services
{

    public interface IUserService
    {

        Task<UserDTOResponse> GetUserByCpf(string cpf);
        Task<List<UserDTOResponse>> GetUsersByNome(string nome);
        Task DesativarUser(string cpf, string tipo);
        Task AtivarUser(string cpf, string tipo);

    }
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;

        }

        private Task<UserDTOResponse?> BuscarAlunoPorCpf(string cpf)
        {
            return _context.Alunos
                .Where(a => a.Cpf == cpf)
                .Select(a => new UserDTOResponse
                {
                    Nome = a.Nome,
                    Email = a.Email,
                    Cpf = a.Cpf,
                    Tipo = "Aluno",
                    Status = a.Status.ToString()
                })
                .FirstOrDefaultAsync();
        }

        private Task<UserDTOResponse?> BuscarProfessorPorCpf(string cpf)
        {
            return _context.Professores
                .Where(p => p.Cpf == cpf)
                .Select(p => new UserDTOResponse
                {
                    Nome = p.Nome,
                    Email = p.Email,
                    Cpf = p.Cpf,
                    Tipo = "Professor",
                    Status = p.Status.ToString()
                })
                .FirstOrDefaultAsync();
        }

        private Task<List<UserDTOResponse>> BuscarAlunosPorNome(string nome)
        {
            return _context.Alunos
                .Where(a => a.Nome.ToLower().Contains(nome.ToLower()))
                .Select(a => new UserDTOResponse
                {
                    Nome = a.Nome,
                    Email = a.Email,
                    Cpf = a.Cpf,
                    Tipo = "Aluno",
                    Status = a.Status.ToString()
                })
                .ToListAsync();
        }

        private Task<List<UserDTOResponse>> BuscarProfessoresPorNome(string nome)
        {
            return _context.Professores
                .Where(p => p.Nome.ToLower().Contains(nome.ToLower()))
                .Select(p => new UserDTOResponse
                {
                    Nome = p.Nome,
                    Email = p.Email,
                    Cpf = p.Cpf,
                    Tipo = "Professor",
                    Status = p.Status.ToString()
                })
                .ToListAsync();
        }


        public async Task<UserDTOResponse> GetUserByCpf(string cpf)
        {
            var aluno = await BuscarAlunoPorCpf(cpf);
            if (aluno != null)
                return aluno;

            var professor = await BuscarProfessorPorCpf(cpf);
            if (professor != null)
                return professor;


            throw new NotFoundException("Usuario", $"CPF: {cpf}");
        }

        public async Task<List<UserDTOResponse>> GetUsersByNome(string nome)
        {
            var alunos = await BuscarAlunosPorNome(nome);
            var professores = await BuscarProfessoresPorNome(nome);

            var usuarios = alunos
                .Concat(professores)
                .ToList();

            if (!usuarios.Any())
                throw new NotFoundException("Usuario", $"Nome: {nome}");

            return usuarios;
        }

        public async Task DesativarUser(string cpf, string tipo)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                throw new ValidationException("CPF inválido.");

            if (string.IsNullOrWhiteSpace(tipo))
                throw new ValidationException("Tipo de usuário inválido.");

            tipo = tipo.Trim().ToLower();

            if (tipo == "aluno")
            {
                var aluno = await _context.Alunos
                    .FirstOrDefaultAsync(a => a.Cpf == cpf);

                if (aluno == null)
                    throw new NotFoundException("Aluno", $"CPF: {cpf}");

                aluno.Status = StatusUsuario.DESATIVADO;
            }
            else if (tipo == "professor")
            {
                var professor = await _context.Professores
                    .FirstOrDefaultAsync(p => p.Cpf == cpf);

                if (professor == null)
                    throw new NotFoundException("Professor", $"CPF: {cpf}");

                professor.Status = StatusUsuario.DESATIVADO;
            }
            else
            {
                throw new ValidationException("Tipo de usuário inválido.");
            }

            await _context.SaveChangesAsync();
        }

        public async Task AtivarUser(string cpf, string tipo)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                throw new ValidationException("CPF inválido.");

            if (string.IsNullOrWhiteSpace(tipo))
                throw new ValidationException("Tipo de usuário inválido.");

            tipo = tipo.Trim().ToLower();

            if (tipo == "aluno")
            {
                var aluno = await _context.Alunos
                    .FirstOrDefaultAsync(a => a.Cpf == cpf);

                if (aluno == null)
                    throw new NotFoundException("Aluno", $"CPF: {cpf}");

                aluno.Status = StatusUsuario.ATIVO;
            }
            else if (tipo == "professor")
            {
                var professor = await _context.Professores
                    .FirstOrDefaultAsync(p => p.Cpf == cpf);

                if (professor == null)
                    throw new NotFoundException("Professor", $"CPF: {cpf}");

                professor.Status = StatusUsuario.ATIVO;
            }
            else
            {
                throw new ValidationException("Tipo de usuário inválido.");
            }

            await _context.SaveChangesAsync();
        }

    }

}