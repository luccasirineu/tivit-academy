using FluentAssertions;
using Microsoft.Extensions.Logging;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Enums;
using tivitApi.Exceptions;
using tivitApi.Models;
using tivitApi.Services;
using tivitApi.Tests.Helpers;

namespace tivitApi.Tests.Services
{
    public class MateriaServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly MateriaService _service;
        private readonly ILogger<MateriaService> _logger;

        public MateriaServiceTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _logger = new LoggerFactory().CreateLogger<MateriaService>();
            _service = new MateriaService(_context, _logger);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task CriarMateriaAsync_DeveCriarMateriaComSucesso()
        {
            // Arrange
            var curso = new Curso("Curso Teste", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var dto = new MateriaDTO
            {
                Nome = "Matemática",
                Descricao = "Matemática Básica",
                CursoId = curso.Id
            };

            // Act
            var result = await _service.CriarMateriaAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be("Matemática");
            result.CursoId.Should().Be(curso.Id);
        }

        [Fact]
        public async Task CriarMateriaAsync_DeveLancarExcecao_QuandoCursoNaoExiste()
        {
            // Arrange
            var dto = new MateriaDTO
            {
                Nome = "Matemática",
                Descricao = "Matemática Básica",
                CursoId = 999
            };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CriarMateriaAsync(dto));
        }

        [Fact]
        public async Task GetMateriasByCursoIdAsync_DeveRetornarMateriasDoCurso()
        {
            // Arrange
            var curso = new Curso("Curso Teste", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var materia1 = new Materia("Matemática", "Descrição", curso.Id);
            var materia2 = new Materia("Física", "Descrição", curso.Id);
            _context.Materias.AddRange(materia1, materia2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetMateriasByCursoIdAsync(curso.Id);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetMateriasByCursoIdAsync_DeveLancarExcecao_QuandoCursoNaoExiste()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetMateriasByCursoIdAsync(999));
        }

        [Fact]
        public async Task GetMateriaNomeByMateriaIdAsync_DeveRetornarNomeDaMateria()
        {
            // Arrange
            var curso = new Curso("Curso Teste", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var materia = new Materia("Matemática", "Descrição", curso.Id);
            _context.Materias.Add(materia);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetMateriaNomeByMateriaIdAsync(materia.Id);

            // Assert
            result.Should().Be("Matemática");
        }

        [Fact]
        public async Task GetMateriaNomeByMateriaIdAsync_DeveLancarExcecao_QuandoMateriaNaoExiste()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetMateriaNomeByMateriaIdAsync(999));
        }
    }
}
