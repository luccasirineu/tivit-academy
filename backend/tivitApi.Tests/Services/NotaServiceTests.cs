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
    public class NotaServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly NotaService _service;
        private readonly ILogger<MatriculaService> _logger;

        public NotaServiceTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _logger = new LoggerFactory().CreateLogger<MatriculaService>();
            _service = new NotaService(_context, _logger);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task AdicionarNotaAsync_DeveAdicionarNotaComSucesso()
        {
            // Arrange
            var curso = new Curso("Curso Teste", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var matricula = new Matricula("João", "joao@test.com", "12345678900", curso.Id);
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            var aluno = new Aluno("João", "joao@test.com", "12345678900", "senha123", matricula.Id);
            var materia = new Materia("Matemática", "Descrição", curso.Id);

            _context.Alunos.Add(aluno);
            _context.Materias.Add(materia);
            await _context.SaveChangesAsync();

            var dto = new NotaDTORequest
            {
                AlunoId = aluno.Id,
                MateriaId = materia.Id,
                Nota1 = 8.0m,
                Nota2 = 7.0m
            };

            // Act
            var result = await _service.AdicionarNotaAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.AlunoId.Should().Be(aluno.Id);
            result.MateriaId.Should().Be(materia.Id);
            result.Media.Should().Be(7.5m);
            result.Status.Should().Be(StatusNota.APROVADO.ToString());
        }

        [Fact]
        public async Task AdicionarNotaAsync_DeveLancarExcecao_QuandoAlunoNaoExiste()
        {
            // Arrange
            var dto = new NotaDTORequest
            {
                AlunoId = 999,
                MateriaId = 1,
                Nota1 = 8.0m,
                Nota2 = 7.0m
            };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.AdicionarNotaAsync(dto));
        }

        [Fact]
        public async Task GetAllNotasByAlunoId_DeveLancarExcecao_QuandoAlunoNaoExiste()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetAllNotasByAlunoId(999));
        }
    }
}
