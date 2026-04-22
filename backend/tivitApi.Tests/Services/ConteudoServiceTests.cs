using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Enums;
using tivitApi.Exceptions;
using tivitApi.Models;
using tivitApi.Services;
using tivitApi.Tests.Helpers;

namespace tivitApi.Tests.Services
{
    public class ConteudoServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly Mock<ILogger<ConteudoService>> _mockLogger;
        private readonly Mock<IGeminiService> _mockGeminiService;
        private readonly ConteudoService _service;

        public ConteudoServiceTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _mockLogger = new Mock<ILogger<ConteudoService>>();
            _mockGeminiService = new Mock<IGeminiService>();

            _service = new ConteudoService(
                _context,
                _mockLogger.Object,
                _mockGeminiService.Object
            );
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task CriarConteudoLinkAsync_DeveCriarConteudoComSucesso()
        {
            // Arrange
            var curso = new Curso("Curso Teste", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var materia = new Materia("Matemática", "Descrição", curso.Id);
            _context.Materias.Add(materia);
            await _context.SaveChangesAsync();

            var turma = new Turma("Turma A", curso.Id, "Ativa");
            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            var professor = new Professor("Maria Santos", "maria@email.com", "RM123456", "98765432100", StatusUsuario.ATIVO)
            {
                Senha = "senhaHash"
            };
            _context.Professores.Add(professor);
            await _context.SaveChangesAsync();

            var dto = new CreateConteudoLinkDTO
            {
                Titulo = "Conteúdo Teste",
                Url = "https://example.com/conteudo",
                MateriaId = materia.Id,
                TurmaId = turma.Id
            };

            // Act
            var result = await _service.CriarConteudoLinkAsync(dto, professor.Id);

            // Assert
            result.Should().NotBeNull();
            result.Titulo.Should().Be("Conteúdo Teste");
            result.Tipo.Should().Be("link");
            result.CaminhoOuUrl.Should().Be("https://example.com/conteudo");
        }

        [Fact]
        public async Task CriarConteudoLinkAsync_DeveLancarExcecao_QuandoUrlInvalida()
        {
            // Arrange
            var dto = new CreateConteudoLinkDTO
            {
                Titulo = "Conteúdo Teste",
                Url = "url-invalida",
                MateriaId = 1,
                TurmaId = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _service.CriarConteudoLinkAsync(dto, 1));
        }

        [Fact]
        public async Task GetConteudosByMateriaIdAsync_DeveRetornarConteudosDaMateria()
        {
            // Arrange
            var curso = new Curso("Curso Teste", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var materia = new Materia("Matemática", "Descrição", curso.Id);
            _context.Materias.Add(materia);
            await _context.SaveChangesAsync();

            var turma = new Turma("Turma A", curso.Id, "Ativa");
            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            var conteudo1 = new Conteudo
            {
                Titulo = "Conteúdo 1",
                Tipo = "link",
                CaminhoOuUrl = "https://example.com/1",
                MateriaId = materia.Id,
                TurmaId = turma.Id,
                ProfessorId = 1,
                DataPublicacao = DateTime.UtcNow
            };

            var conteudo2 = new Conteudo
            {
                Titulo = "Conteúdo 2",
                Tipo = "link",
                CaminhoOuUrl = "https://example.com/2",
                MateriaId = materia.Id,
                TurmaId = turma.Id,
                ProfessorId = 1,
                DataPublicacao = DateTime.UtcNow
            };

            _context.Conteudos.AddRange(conteudo1, conteudo2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetConteudosByMateriaIdAsync(materia.Id, turma.Id);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetConteudosByMateriaIdAsync_DeveLancarExcecao_QuandoMateriaNaoExiste()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetConteudosByMateriaIdAsync(999, 1));
        }

        [Fact]
        public async Task GetConteudosByMateriaIdAsync_DeveRetornarListaVazia_QuandoNaoHaConteudos()
        {
            // Arrange
            var curso = new Curso("Curso Teste", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var materia = new Materia("Matemática", "Descrição", curso.Id);
            _context.Materias.Add(materia);
            await _context.SaveChangesAsync();

            var turma = new Turma("Turma A", curso.Id, "Ativa");
            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetConteudosByMateriaIdAsync(materia.Id, turma.Id);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
