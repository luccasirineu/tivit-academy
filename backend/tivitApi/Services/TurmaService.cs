using Microsoft.EntityFrameworkCore;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Exceptions;

namespace tivitApi.Services
{
    public interface ITurmaService
    {
        Task CriarTurma(TurmaDTORequest turmaDto);
        Task<List<TurmaDTOResponse>> GetTurmasByCursoId(int cursoId);

    }


    public class TurmaService : ITurmaService
    {
        private readonly AppDbContext _context;

        public TurmaService(AppDbContext context)
        {
            _context = context;
        }

        private Turma ConvertTurmaDtoToTurma(TurmaDTORequest turmaDto)
        {
            return new Turma(
                turmaDto.Nome,
                turmaDto.CursoId
                );
        }

        private TurmaDTOResponse ConvertTurmaToTurmaDto(Turma turma)
        {
            return new TurmaDTOResponse(
                turma.Id,
                turma.Nome,
                turma.CursoId
                );
        }

        public async Task CriarTurma(TurmaDTORequest turmaDto)
        {

            var turma = ConvertTurmaDtoToTurma(turmaDto);

            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

        }

        public async Task<List<TurmaDTOResponse>> GetTurmasByCursoId(int cursoId)
        {
            var cursoExiste = await _context.Cursos.AnyAsync(c => c.Id == cursoId);
            if (!cursoExiste)
                throw new Exception("Curso não encontrado");

            var turmas = await _context.Turmas
                .Where(m => m.CursoId == cursoId)
                .OrderBy(m => m.Nome)
                .ToListAsync();

            var resultado = turmas
            .Select(turma => ConvertTurmaToTurmaDto(turma))
            .ToList();

            return resultado;
        }


    }
}

