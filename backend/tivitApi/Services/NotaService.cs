using Microsoft.EntityFrameworkCore;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Exceptions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace tivitApi.Services
{

    public interface INotaService
    {
        Task<NotaDTOResponse> AdicionarNotaAsync(NotaDTORequest notaDTO);
        Task<List<NotaDTOResponse>> GetAllNotasByAlunoId(int alunoId);
        Task<DesempenhoDTO> GetDesempenhoByAlunoId(int alunoId);
        Task<List<NotaDTOResponse>> GetAllNotasByMatriculaId(int matriculaId);
        Task<List<NotaDTOResponse>> GetAllNotasByNomeAluno(string nome);
        Task<byte[]> GerarRelatorioNotasPdfAsync(int alunoId);
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
                    _logger.LogWarning("Aluno não encontrado. AlunoId: {AlunoId}", dto.AlunoId);
                    throw new NotFoundException("Aluno", dto.AlunoId);
                }

                var materiaExiste = await _context.Materias
                    .AnyAsync(m => m.Id == dto.MateriaId);

                if (!materiaExiste)
                {
                    _logger.LogWarning("Matéria não encontrada. MateriaId: {MateriaId}", dto.MateriaId);
                    throw new NotFoundException("Materia", dto.MateriaId);
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
                    throw new ValidationException("Notas devem estar entre 0 e 10.");
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

                    throw new BusinessException(
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
            catch (Exception ex) when (ex is not NotFoundException && ex is not ValidationException && ex is not BusinessException)
            {
                _logger.LogError(ex, "Erro inesperado ao adicionar nota. Payload: {@NotaDTO}", dto);
                throw new BusinessException($"Erro ao adicionar nota: {ex.Message}");
            }
        }

        public async Task<List<NotaDTOResponse>> GetAllNotasByAlunoId(int alunoId)
        {
            // Valida se o aluno existe
            var alunoExiste = await _context.Alunos.AnyAsync(a => a.Id == alunoId);
            if (!alunoExiste)
                throw new NotFoundException("Aluno", alunoId);

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
                throw new NotFoundException("Nota", $"AlunoId: {alunoId}");

            var materia = await _context.Materias
                .Where(m => m.Id == notasAluno.MateriaId)
                .Select(m => m.Nome)
                .FirstOrDefaultAsync();

            if (materia == null)
                throw new NotFoundException("Materia", notasAluno.MateriaId);

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
                throw new NotFoundException("Aluno", $"MatriculaId: {matriculaId}");

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
                throw new NotFoundException("Nota", $"Nome do aluno: {nome}");

            return notas;
        }

        public async Task<byte[]> GerarRelatorioNotasPdfAsync(int alunoId)
        {
           
            var notas = await _context.Notas
            .Include(n => n.Materia)
            .Where(n => n.AlunoId == alunoId)
            .ToListAsync();

            var aluno = await _context.Alunos.FindAsync(alunoId);

            var mediaGeral = notas.Any() ? notas.Average(n => n.Media) : 0;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("TIVIT Academy | Relatório Acadêmico")
                            .FontSize(20).Bold().FontColor(Color.FromHex("#ff0054"));
                        col.Item().Text($"Aluno: {aluno?.Nome ?? ""}").FontSize(13);
                        col.Item().Text($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10).FontColor(Colors.Grey.Medium);
                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    page.Content().PaddingTop(20).Column(col =>
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(3);
                                cols.RelativeColumn();
                                cols.RelativeColumn();
                                cols.RelativeColumn();
                                cols.RelativeColumn();
                                cols.RelativeColumn();
                            });

                            // Header 
                            table.Header(h =>
                            {
                                foreach (var header in new[] { "Matéria", "Nota 1", "Nota 2", "Média", "Faltas", "Status" })
                                {
                                    h.Cell().Background(Color.FromHex("#ff0054"))
                                        .Padding(6)
                                        .Text(header).FontColor(Colors.White).Bold();
                                }
                            });

                            // Linhas
                            foreach (var nota in notas)
                            {
                                var isAprovado = nota.Status == "APROVADO";
                                table.Cell().Padding(6).Text(nota.Materia?.Nome ?? "");
                                table.Cell().Padding(6).Text(nota.Nota1.ToString("F2"));
                                table.Cell().Padding(6).Text(nota.Nota2.ToString("F2"));
                                table.Cell().Padding(6).Text(nota.Media.ToString("F2"));
                                table.Cell().Padding(6).Text(nota.QtdFaltas.ToString());
                                table.Cell().Padding(6).Text(nota.Status)
                                    .FontColor(isAprovado ? Color.FromHex("#00aa55") : Color.FromHex("#ff0054"));
                            }
                        });

                        col.Item().PaddingTop(20)
                            .Text($"Média Geral: {mediaGeral:F2}")
                            .FontSize(14).Bold();
                    });

                    page.Footer().AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                            x.Span(" de ");
                            x.TotalPages();
                        });
                });
            });

            return document.GeneratePdf();
            
        }
    }
}