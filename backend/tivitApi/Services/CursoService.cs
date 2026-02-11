using tivitApi.Data;
using tivitApi.Models;
using tivitApi.DTOs;
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

        private CursoDTO ConvertCursoToCursoDTO(Curso curso)
        {
            return new CursoDTO(
                curso.Id,
                curso.Nome,
                curso.Descricao,
                curso.ProfResponsavel,
                curso.Status
                );
        }
        
        private Curso ConvertCursoDTOToCurso(CursoDTORequest dto)
        {
            return new Curso(
                dto.Nome,
                dto.Descricao,
                dto.ProfResponsavel,
                "ATIVO"
                );
        }

        public async Task<List<CursoDTO>> GetAllCursosAsync()
        {
            var cursos = await _context.Cursos.ToListAsync();

            // converter lista de Matricula -> lista de MatriculaDTO
            var resultado = cursos
            .Select(curso => ConvertCursoToCursoDTO(curso))
            .ToList();

            return resultado;
        }

        public async Task<CursoDTO> GetCursoById(int cursoId)
        {
            var curso = await _context.Cursos.FindAsync(cursoId);
            if (curso == null)
                throw new Exception("Curso não encontrado.");

            CursoDTO cursoDTO =  ConvertCursoToCursoDTO(curso);
            return cursoDTO;
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
                throw new Exception("Erro interno ao buscar quantidade de cursos relacionados ao professor.");
            }
        }

        public async Task<List<CursoDTO>> GetAllCursosProfAsync(int professorId)
        {
            var cursos = await _context.Cursos.Where(m => m.ProfResponsavel == professorId).ToListAsync();

            var resultado = cursos
            .Select(curso => ConvertCursoToCursoDTO(curso))
            .ToList();

            return resultado;
        }

        public async Task<int> GetQntdAlunosByCursoId(int cursoId)
        {
            return await _context.Matriculas
                .Where(m => m.CursoId == cursoId && m.Status == "APROVADO")
                .CountAsync();
        }
    
        public async Task CriarCurso(CursoDTORequest dto)
        {
            var curso = ConvertCursoDTOToCurso(dto);
            _context.Cursos.AddRange(curso);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarCurso(CursoDTORequest dto)
        {
            if (dto == null || dto.Id <= 0)
                return;

            var curso = await _context.Cursos
                .FirstOrDefaultAsync(c => c.Id == dto.Id);

            if (curso == null)
                throw new Exception("Curso não encontrado.");

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
                throw new Exception("Curso não encontrado.");

            curso.Status = "DESATIVADO";
            
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
                throw new Exception("Curso não encontrado.");

            curso.Status = "ATIVO";

            _context.Cursos.Update(curso);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CursoDTO>> GetAllCursosAtivos()
        {

            var cursos = await _context.Cursos.Where(p => p.Status == "ATIVO").ToListAsync();

            // converter lista de Matricula -> lista de MatriculaDTO
            var resultado = cursos
            .Select(curso => ConvertCursoToCursoDTO(curso))
            .ToList();

            return resultado;

        }
    }
}
    