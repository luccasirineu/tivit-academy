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

            // converter lista de Matricula → lista de MatriculaDTO
            var resultado = cursos
            .Select(curso => ConvertCursoToCursoDTO(curso))
            .ToList();

            return resultado;
        }

    }
}
    