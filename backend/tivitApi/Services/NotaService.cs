using Microsoft.EntityFrameworkCore;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Exceptions;

namespace tivitApi.Services
{

    public interface INotaService
    {
        Task<NotaDTOResponse> AdicionarNotaAsync(NotaDTORequest notaDTO);
        Task<List<NotaDTOResponse>> GetAllNotasByAlunoId(int alunoId);
        Task<DesempenhoDTO> GetDesempenhoByAlunoId(int alunoId);
        Task<List<NotaDTOResponse>> GetAllNotasByMatriculaId(int matriculaId);
        Task<List<NotaDTOResponse>> GetAllNotasByNomeAluno(string nome);
    }

    public class NotaService : INotaService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MatriculaService> _logger;

        public NotaService(AppDbContext context, ILogger<MatriculaService> logger)
        {
            _context = context;
            _logger = logger;

        }

        private Nota ConvertNotaDtoToNota(NotaDTORequest notaDTO, decimal media, string status, int qntdFaltas)
        {
            return new Nota(
                notaDTO.AlunoId,
                notaDTO.MateriaId,
                notaDTO.Nota1,
                notaDTO.Nota2,
                media,
                qntdFaltas,
                status
                );
        }


        private decimal CalcularMedia(decimal nota1, decimal nota2)
        {

            var media = (nota1 + nota2) / 2;

            return media;
        }

        private string CalcularStatusNota(decimal media, int qntdFaltas)
        {
            if (qntdFaltas > 10) return "REPROVADO";

            if (media >= 6) return "APROVADO";
            return "REPROVADO";
        }

        private async Task<int> ObterFaltas(int alunoId, int materiaId)
        {
            var matriculaId = await _context.Alunos
                .Where(a => a.Id == alunoId)
                .Select(a => a.MatriculaId)
                .FirstOrDefaultAsync();

            if (matriculaId == 0)
                return 0;

            var totalFaltas = await _context.Chamadas
                .CountAsync(c => c.MatriculaId == matriculaId && c.Faltou && c.MateriaId == materiaId);

            return totalFaltas;
        }

        public async Task<NotaDTOResponse> AdicionarNotaAsync(NotaDTORequest dto)
        {
            try
            {
                _logger.LogInformation(
                    "Iniciando cadastro de nota. AlunoId: {AlunoId}, MateriaId: {MateriaId}",
                    dto.AlunoId,
                    dto.MateriaId
                );

                var alunoExiste = await _context.Alunos
                    .AnyAsync(a => a.Id == dto.AlunoId);

                if (!alunoExiste)
                {
                    _logger.LogWarning(
                        "Aluno não encontrado. AlunoId: {AlunoId}",
                        dto.AlunoId
                    );
                    throw new Exception("Aluno não encontrado.");
                }

                var materiaExiste = await _context.Materias
                    .AnyAsync(m => m.Id == dto.MateriaId);

                if (!materiaExiste)
                {
                    _logger.LogWarning(
                        "Matéria não encontrada. MateriaId: {MateriaId}",
                        dto.MateriaId
                    );
                    throw new Exception("Matéria não encontrada.");
                }

                if (dto.Nota1 < 0 || dto.Nota1 > 10 ||
                    dto.Nota2 < 0 || dto.Nota2 > 10)
                {
                    _logger.LogWarning(
                        "Notas inválidas informadas. Nota1: {Nota1}, Nota2: {Nota2}, AlunoId: {AlunoId}",
                        dto.Nota1,
                        dto.Nota2,
                        dto.AlunoId
                    );
                    throw new Exception("Notas devem estar entre 0 e 10.");
                }

                var qntdFaltas = await ObterFaltas(dto.AlunoId, dto.MateriaId);

                decimal media = CalcularMedia(dto.Nota1, dto.Nota2);
                string status = CalcularStatusNota(media, qntdFaltas);

                var nota = ConvertNotaDtoToNota(dto, media, status, qntdFaltas);

                _context.Notas.Add(nota);

                try
                {
                    await _context.SaveChangesAsync();

                    _logger.LogInformation(
                        "Nota salva com sucesso. AlunoId: {AlunoId}, MateriaId: {MateriaId}, Media: {Media}, Faltas: {Faltas}",
                        nota.AlunoId,
                        nota.MateriaId,
                        media,
                        qntdFaltas
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Erro ao salvar nota no banco. AlunoId: {AlunoId}, MateriaId: {MateriaId}",
                        dto.AlunoId,
                        dto.MateriaId
                    );

                    throw new Exception(
                        "Erro ao salvar a nota. Verifique se já existe cadastro para este aluno e matéria."
                    );
                }

                return new NotaDTOResponse
                {
                    AlunoId = nota.AlunoId,
                    MateriaId = nota.MateriaId,
                    Nota1 = nota.Nota1,
                    Nota2 = nota.Nota2,
                    Media = media,
                    QtdFaltas = qntdFaltas,
                    Status = status
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro inesperado ao adicionar nota. Payload: {@NotaDTO}",
                    dto
                );

                throw;
            }
        }

        public async Task<List<NotaDTOResponse>> GetAllNotasByAlunoId(int alunoId)
        {
            // Valida se o aluno existe
            var alunoExiste = await _context.Alunos.AnyAsync(a => a.Id == alunoId);
            if (!alunoExiste)
                throw new Exception("Aluno não encontrado.");

            var notas = await _context.Notas
                .Where(n => n.AlunoId == alunoId)
                .Select(n => new NotaDTOResponse
                {
                    AlunoId = n.AlunoId,
                    MateriaId = n.MateriaId,
                    Nota1 = n.Nota1,
                    Nota2 = n.Nota2,
                    Media = n.Media,
                    QtdFaltas = n.QtdFaltas,
                    Status = n.Status
                })
                .ToListAsync();

            return notas;
        }

        public async Task<DesempenhoDTO> GetDesempenhoByAlunoId(int alunoId)
        {
            var notasAluno = await _context.Notas
                .Where(n => n.AlunoId == alunoId)
                .OrderByDescending(n => n.Media)
                .FirstOrDefaultAsync();

            if (notasAluno == null)
                throw new Exception("Nenhuma nota encontrada para este aluno.");

            var materia = await _context.Materias
                .Where(m => m.Id == notasAluno.MateriaId)
                .Select(m => m.Nome)
                .FirstOrDefaultAsync();

            if (materia == null)
                throw new Exception("Matéria não encontrada.");

            string nivel;

            if (notasAluno.Media < 6)
                nivel = "BRONZE";
            else if (notasAluno.Media <= 8)
                nivel = "PRATA";
            else
                nivel = "OURO";

            return new DesempenhoDTO
            {
                NomeMateria = materia,
                Media = notasAluno.Media,
                QtdFaltas = notasAluno.QtdFaltas,
                Nivel = nivel
            };
        }

        public async Task<List<NotaDTOResponse>> GetAllNotasByMatriculaId(int matriculaId)
        {
            // Valida se o aluno existe
            var alunoId = await _context.Alunos
                .Where(a => a.MatriculaId == matriculaId)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();
            
            if (alunoId == 0)
                throw new Exception("Aluno não encontrado.");

            var notas = await _context.Notas
                .Where(n => n.AlunoId == alunoId)
                .Select(n => new NotaDTOResponse
                {
                    AlunoId = n.AlunoId,
                    MateriaId = n.MateriaId,
                    Nota1 = n.Nota1,
                    Nota2 = n.Nota2,
                    Media = n.Media,
                    QtdFaltas = n.QtdFaltas,
                    Status = n.Status
                })
                .ToListAsync();

            return notas;
        }

        public async Task<List<NotaDTOResponse>> GetAllNotasByNomeAluno(string nome)
        {
            var notas = await _context.Notas
                .Include(n => n.Aluno)
                .Where(n => n.Aluno.Nome.Contains(nome))
                .Select(n => new NotaDTOResponse
                {
                    AlunoId = n.AlunoId,
                    MateriaId = n.MateriaId,
                    Nota1 = n.Nota1,
                    Nota2 = n.Nota2,
                    Media = n.Media,
                    QtdFaltas = n.QtdFaltas,
                    Status = n.Status
                })
                .ToListAsync();

            if (!notas.Any())
                throw new Exception("Nenhuma nota encontrada para este aluno.");

            return notas;
        }

    }
}