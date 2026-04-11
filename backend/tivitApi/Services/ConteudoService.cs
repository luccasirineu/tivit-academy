using Microsoft.EntityFrameworkCore;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Exceptions;
using tivitApi.Mappers;

namespace tivitApi.Services
{
    public interface IConteudoService
    {
        Task<Conteudo> CriarConteudoPdfAsync(CreateConteudoPdfDTO dto, int professorId);
        Task<Conteudo> CriarConteudoLinkAsync(CreateConteudoLinkDTO dto, int professorId);
        Task<List<ConteudoDTO>> GetConteudosByMateriaIdAsync(int materiaId, int turmaId);
    }

    public class ConteudoService : IConteudoService
    { 
        private readonly AppDbContext _context;
        private readonly ILogger<ConteudoService> _logger;
        private readonly IGeminiService _geminiService;

        public ConteudoService(
            AppDbContext context, 
            ILogger<ConteudoService> logger,
            IGeminiService geminiService)
        {
            _context = context;
            _logger = logger;
            _geminiService = geminiService;
        }


        public async Task<Conteudo> CriarConteudoPdfAsync(CreateConteudoPdfDTO dto, int professorId)
        {
            if (dto.Arquivo == null || dto.Arquivo.Length == 0)
                throw new ValidationException("Arquivo inválido");

            var pasta = Path.Combine("wwwroot", "uploads", $"turma_{dto.TurmaId}", $"materia_{dto.MateriaId}");
            Directory.CreateDirectory(pasta);

            var nomeArquivo = $"{Guid.NewGuid()}{Path.GetExtension(dto.Arquivo.FileName)}";
            var caminhoCompleto = Path.Combine(pasta, nomeArquivo);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await dto.Arquivo.CopyToAsync(stream);
            }

            var conteudo = new Conteudo
            {
                Titulo = dto.Titulo,
                Tipo = "pdf",
                CaminhoOuUrl = $"/uploads/turma_{dto.TurmaId}/materia_{dto.MateriaId}/{nomeArquivo}",
                MateriaId = dto.MateriaId,
                ProfessorId = professorId,
                DataPublicacao = DateTime.UtcNow,
                TurmaId = dto.TurmaId
            };

            _context.Conteudos.Add(conteudo);
            await _context.SaveChangesAsync();

            // Processar conteúdo com a IA e armazenar contexto
            try
            {
                _logger.LogInformation("Iniciando processamento de contexto para PDF: {ConteudoId}", conteudo.Id);
                var conteudoContexto = await _geminiService.ExtrairEProcessarConteudoAsync(
                    conteudo,
                    caminhoCompleto,
                    "pdf");

                _context.ConteudosContexto.Add(conteudoContexto);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Contexto armazenado com sucesso para ConteudoId: {ConteudoId}", conteudo.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar contexto do PDF. Conteúdo salvo sem contexto.");
                // Não falhar o upload se houver erro no processamento de IA
            }

            return conteudo;
        }

        public async Task<Conteudo> CriarConteudoLinkAsync(CreateConteudoLinkDTO dto, int professorId)
        {
            if (!Uri.IsWellFormedUriString(dto.Url, UriKind.Absolute))
                throw new ValidationException("URL inválida");

            var conteudo = new Conteudo
            {
                Titulo = dto.Titulo,
                Tipo = "link",
                CaminhoOuUrl = dto.Url,
                MateriaId = dto.MateriaId,
                ProfessorId = professorId,
                DataPublicacao = DateTime.UtcNow,
                TurmaId = dto.TurmaId
            };

            _context.Conteudos.Add(conteudo);
            await _context.SaveChangesAsync();

            // Processar conteúdo com a IA e armazenar contexto
            try
            {
                _logger.LogInformation("Iniciando processamento de contexto para Link: {ConteudoId}", conteudo.Id);
                var conteudoContexto = await _geminiService.ExtrairEProcessarConteudoAsync(
                    conteudo,
                    dto.Url,
                    "link");

                _context.ConteudosContexto.Add(conteudoContexto);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Contexto armazenado com sucesso para ConteudoId: {ConteudoId}", conteudo.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar contexto do Link. Conteúdo salvo sem contexto.");
                // Não falhar o upload se houver erro no processamento de IA
            }

            return conteudo;
        }

        public async Task<List<ConteudoDTO>> GetConteudosByMateriaIdAsync(int materiaId, int turmaId)
        {
            var materiaExiste = await _context.Materias.AnyAsync(c => c.Id == materiaId);
            if (!materiaExiste)
                throw new NotFoundException("Materia", materiaId);

            var conteudos = await _context.Conteudos
                .Where(m => m.MateriaId == materiaId && m.TurmaId == turmaId)
                .ToListAsync();

            return conteudos.Select(conteudo => conteudo.ToDTO()).ToList();
        }
    }
}