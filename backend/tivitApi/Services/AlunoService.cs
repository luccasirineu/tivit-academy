using Microsoft.EntityFrameworkCore;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Exceptions;

namespace tivitApi.Services
{
    public interface IAlunoService
    {
        Task<AlunoDTO> getInfoAluno(int alunoId);

    }

    public class AlunoService : IAlunoService
    {
        private readonly AppDbContext _context;

        public AlunoService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<AlunoDTO> getInfoAluno(int alunoId)
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

    }
  }