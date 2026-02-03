using Microsoft.EntityFrameworkCore;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Exceptions;

namespace tivitApi.Services
{

    public interface IUserService
    {

        Task<UserDTOResponse> GetUserByCpf(string cpf);
        Task<List<UserDTOResponse>> GetUsersByNome(string nome);

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
                    Cpf = a.Cpf
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
                    Cpf = p.Cpf
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
                    Cpf = a.Cpf
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
                    Cpf = p.Cpf
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


            throw new Exception("Usuário não encontrado.");
        }

        public async Task<List<UserDTOResponse>> GetUsersByNome(string nome)
        {
            var alunos = await BuscarAlunosPorNome(nome);
            var professores = await BuscarProfessoresPorNome(nome);

            var usuarios = alunos
                .Concat(professores)
                .ToList();

            if (!usuarios.Any())
                throw new Exception("Usuário não encontrado.");

            return usuarios;
        }
    }
    
}