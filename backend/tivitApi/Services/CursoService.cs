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

        public CursoDTO ConvertCursoToCursoDTO(Curso curso)
        {
            return new CursoDTO(
                curso.Id,
                curso.Nome,
                curso.Descricao,
                curso.ProfResponsavel
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


    }
}
    