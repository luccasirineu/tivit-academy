using tivitApi.Data;
using tivitApi.Models;
using tivitApi.DTOs;
using tivitApi.Exceptions;
using tivitApi.Mappers;
using tivitApi.Enums;
using Microsoft.EntityFrameworkCore;

namespace tivitApi.Services
{
    public class CursoService : ICursoService
    {
        private readonly AppDbContext _context;

        public CursoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CursoDTO>> GetAllCursosAsync()
        {
            var cursos = await _context.Cursos.ToListAsync();

            return cursos.Select(curso => curso.ToDTO()).ToList();
        }

        public async Task<CursoDTO> GetCursoById(int cursoId)
        {
            var curso = await _context.Cursos.FindAsync(cursoId);
            if (curso == null)
                throw new NotFoundException("Curso", cursoId);

            return curso.ToDTO();
        }

        public async Task<int> GetQntdCursosProf(int professorId)
        {
            try
            {
                var quantidade = await _context.Cursos.Where(m => m.ProfResponsavel == professorId).CountAsync();

                return quantidade;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Erro interno ao buscar quantidade de cursos relacionados ao professor.");
            }
        }

        public async Task<List<CursoDTO>> GetAllCursosProfAsync(int professorId)
        {
            var cursos = await _context.Cursos.Where(m => m.ProfResponsavel == professorId).ToListAsync();

            return cursos.Select(curso => curso.ToDTO()).ToList();
        }

        public async Task<int> GetQntdAlunosByCursoId(int cursoId)
        {
            return await _context.Matriculas
                .Where(m => m.CursoId == cursoId && m.Status == StatusMatricula.APROVADO)
                .CountAsync();
        }
    
        public async Task CriarCurso(CursoDTORequest dto)
        {
            var curso = dto.ToEntity();
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarCurso(CursoDTORequest dto)
        {
            if (dto == null || dto.Id <= 0)
                return;

            var curso = await _context.Cursos
                .FirstOrDefaultAsync(c => c.Id == dto.Id);

            if (curso == null)
                throw new NotFoundException("Curso", dto.Id);

            curso.Nome = dto.Nome;
            curso.Descricao = dto.Descricao;
            curso.ProfResponsavel = dto.ProfResponsavel;

            _context.Cursos.Update(curso);
            await _context.SaveChangesAsync();
        }

        public async Task DesativarCurso(int cursoId)
        {
            if (cursoId == null || cursoId <= 0)
                return;

            var curso = await _context.Cursos
                .FirstOrDefaultAsync(c => c.Id == cursoId);

            if (curso == null)
                throw new NotFoundException("Curso", cursoId);

            curso.Status = StatusCurso.DESATIVADO;
            
            _context.Cursos.Update(curso);
            await _context.SaveChangesAsync();
        }

        public async Task AtivarCurso(int cursoId)
        {
            if (cursoId == null || cursoId <= 0)
                return;

            var curso = await _context.Cursos
                .FirstOrDefaultAsync(c => c.Id == cursoId);

            if (curso == null)
                throw new NotFoundException("Curso", cursoId);

            curso.Status = StatusCurso.ATIVO;

            _context.Cursos.Update(curso);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CursoDTO>> GetAllCursosAtivos()
        {
            var cursos = await _context.Cursos.Where(p => p.Status == StatusCurso.ATIVO).ToListAsync();

            return cursos.Select(curso => curso.ToDTO()).ToList();
        }
    }
}
    