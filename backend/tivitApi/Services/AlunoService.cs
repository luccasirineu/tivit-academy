using Microsoft.EntityFrameworkCore;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Exceptions;

namespace tivitApi.Services
{
    public interface IAlunoService
    {
        Task<AlunoDTO> GetInfoAluno(int alunoId);
        Task<List<AlunoDTO>> GetAllAlunosByCurso(int cursoId);
    }

    public class AlunoService : IAlunoService
    {
        private readonly AppDbContext _context;

        public AlunoService(AppDbContext context)
        {
            _context = context;
        }

        private AlunoDTO ConvertAtributosToAlunoDTO(string Nome, string Email, string Cpf, int Id)
        {
            return new AlunoDTO(
                Nome,
                Email,
                Cpf,
                Id
                );
        }

        public async Task<AlunoDTO> GetInfoAluno(int alunoId)
        {
            var aluno = await _context.Alunos
                .Where(a => a.Id == alunoId)
                .Select(a => new
                {
                    a.Id,
                    a.Nome,
                    a.Email,
                    a.Cpf,
                    a.MatriculaId
                })
                .FirstOrDefaultAsync();

            if (aluno == null)
                throw new Exception("Aluno não encontrado.");

            var matricula = await _context.Matriculas
                .Where(m => m.Id == aluno.MatriculaId)
                .Select(m => m.CursoId)
                .FirstOrDefaultAsync();

            if (matricula == 0)
                throw new Exception("Matrícula não encontrada.");

            var cursoNome = await _context.Cursos
                .Where(c => c.Id == matricula)
                .Select(c => c.Nome)
                .FirstOrDefaultAsync();

            if (cursoNome == null)
                throw new Exception("Curso não encontrado.");

            return new AlunoDTO
            {
                Nome = aluno.Nome,
                Email = aluno.Email,
                Cpf = aluno.Cpf,
                MatriculaId = aluno.MatriculaId,
                CursoNome = cursoNome
            };
        }

        public async Task<List<AlunoDTO>> GetAllAlunosByCurso(int cursoId)
        {
            var alunosByCurso = await _context.Matriculas
                .Where(a => a.CursoId == cursoId && a.Status == "APROVADO")
                .Select(a => new
                {
                    a.Nome,
                    a.Email,
                    a.Cpf,
                    a.Id
                })
                .ToListAsync();

            if (alunosByCurso == null)
                throw new Exception("Matriculas não encontradas.");

            var listaAlunos = alunosByCurso
            .Select(matricula => ConvertAtributosToAlunoDTO(matricula.Nome, matricula.Email, matricula.Cpf, matricula.Id))
            .ToList();

            return listaAlunos;
        }

    }
}