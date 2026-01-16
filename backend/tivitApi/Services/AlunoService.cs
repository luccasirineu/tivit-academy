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

        private AlunoDTO ConvertAtributosToAlunoDTO(string Nome, string Email, string Cpf, int Id, int alunoId)
        {
            return new AlunoDTO(
                Nome,
                Email,
                Cpf,
                Id,
                alunoId
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
            var alunosByCurso = await (
                from m in _context.Matriculas
                join a in _context.Alunos on m.Id equals a.MatriculaId
                where m.CursoId == cursoId && m.Status == "APROVADO"
                select new
                {
                    MatriculaId = m.Id,
                    AlunoId = a.Id,
                    m.Nome,
                    m.Email,
                    m.Cpf
                }
            ).ToListAsync();

            if (!alunosByCurso.Any())
                throw new Exception("Matrículas não encontradas.");

            return alunosByCurso
                .Select(x => ConvertAtributosToAlunoDTO(
                    x.Nome,
                    x.Email,
                    x.Cpf,
                    x.MatriculaId,
                    x.AlunoId
                ))
                .ToList();
        }

    }
}