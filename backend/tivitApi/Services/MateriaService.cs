using Microsoft.EntityFrameworkCore;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Exceptions;

namespace tivitApi.Services
{
    public interface IMateriaService
    {
        Task<Materia> CriarMateriaAsync(MateriaDTO materiaDTO);
    }


    public class MateriaService : IMateriaService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MateriaService> _logger;

        public MateriaService(AppDbContext context, ILogger<MateriaService> logger)
        {
            _context = context;
            _logger = logger;

        }

        private Materia ConvertMateriaDtoToMateria(MateriaDTO materiaDTO)
        {
            return new Materia(
                materiaDTO.Nome,
                materiaDTO.Descricao,
                materiaDTO.CursoId
                );
        }

        public async Task<Materia> CriarMateriaAsync(MateriaDTO materiaDto)
        {

            _logger.LogInformation($"Criando matéria : {materiaDto.Nome}");

            // 1️ Verifica se o curso existe
            var curso = await _context.Cursos
                .FirstOrDefaultAsync(c => c.Id == materiaDto.CursoId);

            if (curso == null)
                throw new Exception ("Curso não encontrado");


            var materia = ConvertMateriaDtoToMateria(materiaDto);

            // 3️ Salva no banco
            _context.Materias.Add(materia);
            await _context.SaveChangesAsync();

            return materia;
        }
    }
}

