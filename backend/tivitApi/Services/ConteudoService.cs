using Microsoft.EntityFrameworkCore;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Exceptions;

namespace tivitApi.Services
{
    public interface IConteudoService
    {
        Task<Conteudo> CriarConteudoPdfAsync(CreateConteudoPdfDTO dto, int professorId);
        Task<Conteudo> CriarConteudoLinkAsync(CreateConteudoLinkDTO dto, int professorId);
        Task<List<ConteudoDTO>> GetConteudosByMateriaIdAsync(int materiaId);

    }

    public class ConteudoService : IConteudoService
    { 
        private readonly AppDbContext _context;
        private readonly ILogger<ConteudoService> _logger;

        public ConteudoService(AppDbContext context, ILogger<ConteudoService> logger)
        {
            _context = context;
            _logger = logger;

        }

        private ConteudoDTO ConvertConteudoToConteudoDTO(Conteudo conteudo)
        {
            return new ConteudoDTO(
                conteudo.Titulo,
                conteudo.Tipo,
                conteudo.CaminhoOuUrl,
                conteudo.DataPublicacao,
                conteudo.MateriaId,
                conteudo.ProfessorId
                );
        }


        public async Task<Conteudo> CriarConteudoPdfAsync(CreateConteudoPdfDTO dto, int professorId)
        {
            if (dto.Arquivo == null || dto.Arquivo.Length == 0)
                throw new Exception("Arquivo inválido");

            var pasta = Path.Combine("wwwroot", "uploads", $"materia_{dto.MateriaId}");
            Directory.CreateDirectory(pasta);

            var nomeArquivo = $"{Guid.NewGuid()}{Path.GetExtension(dto.Arquivo.FileName)}";
            var caminhoCompleto = Path.Combine(pasta, nomeArquivo);

            using var stream = new FileStream(caminhoCompleto, FileMode.Create);
            await dto.Arquivo.CopyToAsync(stream);

            var conteudo = new Conteudo
            {
                Titulo = dto.Titulo,
                Tipo = "pdf",
                CaminhoOuUrl = $"/uploads/materia_{dto.MateriaId}/{nomeArquivo}",
                MateriaId = dto.MateriaId,
                ProfessorId = professorId,
                DataPublicacao = DateTime.UtcNow
            };

            _context.Conteudos.Add(conteudo);
            await _context.SaveChangesAsync();

            return conteudo;
        }

        public async Task<Conteudo> CriarConteudoLinkAsync(CreateConteudoLinkDTO dto, int professorId)
        {
            if (!Uri.IsWellFormedUriString(dto.Url, UriKind.Absolute))
                throw new Exception("URL inválida");

            var conteudo = new Conteudo
            {
                Titulo = dto.Titulo,
                Tipo = "link",
                CaminhoOuUrl = dto.Url,
                MateriaId = dto.MateriaId,
                ProfessorId = professorId,
                DataPublicacao = DateTime.UtcNow
            };

            _context.Conteudos.Add(conteudo);
            await _context.SaveChangesAsync();

            return conteudo;
        }

        public async Task<List<ConteudoDTO>> GetConteudosByMateriaIdAsync(int materiaId)
        {
            var materiaExiste = await _context.Materias.AnyAsync(c => c.Id == materiaId);
            if (!materiaExiste)
                throw new Exception("Matéria não encontrada");

            var conteudos = await _context.Conteudos.Where(m => m.MateriaId == materiaId).ToListAsync();

            List<ConteudoDTO> conteudosDTO = new List<ConteudoDTO>();

            foreach (var conteudo in conteudos)
            {
                conteudosDTO.Add(ConvertConteudoToConteudoDTO(conteudo));
            }

            return conteudosDTO;

        }
    }
}