using tivitApi.Data;
using tivitApi.Models;
using tivitApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace tivitApi.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly AppDbContext _context;

        public MatriculaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Matricula> CriarMatriculaAsync(Matricula matricula)
        {
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();
            return matricula;
        }

        public Matricula ConvertMatriculaDtoToMatricula(MatriculaDTO matriculaDTO)
        {
            return new Matricula(
                matriculaDTO.Nome,
                matriculaDTO.Email,
                matriculaDTO.Cpf,
                matriculaDTO.CursoId);
               

        }
    }
}
