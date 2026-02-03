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
        Task<List<AlunoDTO>> GetAllAlunosByTurmaId(int turmaId);
        Task<AlunoDTO> GetAlunoByMatriculaId(int matriculaId);
        Task<int> GetQntdAlunosAtivos();

    }

    public class AlunoService : IAlunoService
    {
        private readonly AppDbContext _context;

        public AlunoService(AppDbContext context)
        {
            _context = context;
        }

        private AlunoDTO ConvertAlunoToAlunoDto(Aluno aluno)
        {
            return new AlunoDTO(
                aluno.Nome,
                aluno.Email,
                aluno.Cpf,
                aluno.MatriculaId,
                aluno.Id,
                aluno.TurmaId
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
                throw new Exception("Matrículas não encontradas.");

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
                throw new Exception("Turma não encontrada");

            var alunos = await _context.Alunos
                .Where(a => a.TurmaId == turmaId)
                .OrderBy(m => m.Nome)
                .ToListAsync();

            var resultado = alunos
            .Select(aluno => ConvertAlunoToAlunoDto(aluno))
            .ToList();

            return resultado;
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
                    a.MatriculaId,
                    a.TurmaId
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
                CursoNome = cursoNome,
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
                throw new Exception("Aluno não encontrado.");

            

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

    }
}