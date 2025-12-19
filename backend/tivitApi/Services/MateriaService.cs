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
        Task<List<Materia>> GetMateriasByCursoIdAsync(int cursoId);
        Task<int> GetCursoIdByAlunoIdAsync(int alunoId);
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

        public async Task<List<Materia>> GetMateriasByCursoIdAsync(int cursoId)
        {
            var cursoExiste = await _context.Cursos.AnyAsync(c => c.Id == cursoId);
            if (!cursoExiste)
                throw new Exception("Curso não encontrado");

            return await _context.Materias
                .Where(m => m.CursoId == cursoId)
                .OrderBy(m => m.Nome)
                .ToListAsync();
        }

        public async Task<int> GetCursoIdByAlunoIdAsync(int alunoId)
        {
            // 1. Buscar aluno
            var aluno = await _context.Alunos
                .FirstOrDefaultAsync(a => a.Id == alunoId);

            if (aluno == null)
                throw new Exception("Aluno não encontrado.");

            // 2. Buscar matrícula usando MatriculaId
            var matricula = await _context.Matriculas
                .FirstOrDefaultAsync(m => m.Id == aluno.MatriculaId);

            if (matricula == null)
                throw new Exception("Matrícula não encontrada.");

            return matricula.CursoId;
        }
    }
}

