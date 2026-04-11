using Microsoft.EntityFrameworkCore;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Exceptions;
using tivitApi.Mappers;

namespace tivitApi.Services
{
    public interface ITurmaService
    {
        Task CriarTurma(TurmaDTORequest turmaDto);
        Task<List<TurmaDTOResponse>> GetTurmasByCursoId(int cursoId);
        Task<TurmaDTOResponse> GetTurmaByAlunoId(int alunoId);
        Task<int> GetQntdTurmasAtivas();
        Task<List<TurmaDTOResponse>> GetAllTurmas();
        Task AtualizarTurma(TurmaDTORequest dto);
    }


    public class TurmaService : ITurmaService
    {
        private readonly AppDbContext _context;

        public TurmaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CriarTurma(TurmaDTORequest turmaDto)
        {
            var turma = turmaDto.ToEntity();

            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TurmaDTOResponse>> GetTurmasByCursoId(int cursoId)
        {
            var cursoExiste = await _context.Cursos.AnyAsync(c => c.Id == cursoId);
            if (!cursoExiste)
                throw new NotFoundException("Curso", cursoId);

            var turmas = await _context.Turmas
                .Where(m => m.CursoId == cursoId)
                .OrderBy(m => m.Nome)
                .ToListAsync();

            return turmas.Select(turma => turma.ToDTO()).ToList();
        }

        public async Task<TurmaDTOResponse> GetTurmaByAlunoId(int alunoId)
        {
            var aluno = await _context.Alunos
                .FirstOrDefaultAsync(a => a.Id == alunoId);

            if (aluno == null)
                throw new NotFoundException("Aluno", alunoId);

            var turma = await _context.Turmas
                .FirstOrDefaultAsync(t => t.Id == aluno.TurmaId);

            if (turma == null)
                throw new NotFoundException("Turma", aluno.TurmaId);

            return turma.ToDTO();
        }

        public async Task<int> GetQntdTurmasAtivas()
        {
            return await _context.Turmas.CountAsync();

        }

        public async Task<List<TurmaDTOResponse>> GetAllTurmas()
        {
            var turmas = await _context.Turmas.ToListAsync();

            return turmas.Select(turma => turma.ToDTO()).ToList();
        }

        public async Task AtualizarTurma(TurmaDTORequest dto)
        {
            if (dto == null || dto.Id <= 0)
                return;

            var turma = await _context.Turmas
                .FirstOrDefaultAsync(t => t.Id == dto.Id);

            if (turma == null)
                throw new NotFoundException("Turma", dto.Id);

            turma.Nome = dto.Nome;
            turma.CursoId = dto.CursoId;
            turma.Status = dto.Status;

            _context.Turmas.Update(turma);
            await _context.SaveChangesAsync();
        }

    }
}

