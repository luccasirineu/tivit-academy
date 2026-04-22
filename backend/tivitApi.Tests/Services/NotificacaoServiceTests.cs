using FluentAssertions;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Enums;
using tivitApi.Exceptions;
using tivitApi.Models;
using tivitApi.Services;
using tivitApi.Tests.Helpers;

namespace tivitApi.Tests.Services
{
    public class NotificacaoServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly NotificacaoService _service;

        public NotificacaoServiceTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _service = new NotificacaoService(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task CriarNotificacao_DeveCriarNotificacaoComSucesso()
        {
            // Arrange
            var curso = new Curso("Curso Teste", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var turma = new Turma("Turma A", curso.Id, "Ativa");
            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            var dto = new NotificacaoDTORequest
            {
                Titulo = "Notificação Teste",
                Descricao = "Descrição da notificação",
                TurmasIds = new List<int> { turma.Id }
            };

            // Act
            var result = await _service.CriarNotificacao(dto);

            // Assert
            result.Should().NotBeNull();
            result.Titulo.Should().Be("Notificação Teste");
            result.NotificacaoTurmas.Should().HaveCount(1);
        }

        [Fact]
        public async Task CriarNotificacao_DeveLancarExcecao_QuandoTurmaNaoExiste()
        {
            // Arrange
            var dto = new NotificacaoDTORequest
            {
                Titulo = "Notificação Teste",
                Descricao = "Descrição da notificação",
                TurmasIds = new List<int> { 999 }
            };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CriarNotificacao(dto));
        }

        [Fact]
        public async Task CriarNotificacao_DeveCriarNotificacaoParaMultiplasTurmas()
        {
            // Arrange
            var curso = new Curso("Curso Teste", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var turma1 = new Turma("Turma A", curso.Id, "Ativa");
            var turma2 = new Turma("Turma B", curso.Id, "Ativa");
            _context.Turmas.AddRange(turma1, turma2);
            await _context.SaveChangesAsync();

            var dto = new NotificacaoDTORequest
            {
                Titulo = "Notificação Teste",
                Descricao = "Descrição da notificação",
                TurmasIds = new List<int> { turma1.Id, turma2.Id }
            };

            // Act
            var result = await _service.CriarNotificacao(dto);

            // Assert
            result.Should().NotBeNull();
            result.NotificacaoTurmas.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetNotificacoesByTurmaId_DeveRetornarNotificacoesDaTurma()
        {
            // Arrange
            var curso = new Curso("Curso Teste", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var turma = new Turma("Turma A", curso.Id, "Ativa");
            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            var notificacao = new Notificacao("Título", "Descrição");
            notificacao.NotificacaoTurmas = new List<NotificacaoTurma>
            {
                new NotificacaoTurma { TurmaId = turma.Id }
            };
            _context.Notificacoes.Add(notificacao);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetNotificacoesByTurmaId(turma.Id);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(1);
            result[0].Titulo.Should().Be("Título");
        }

        [Fact]
        public async Task GetNotificacoesByTurmaId_DeveRetornarListaVazia_QuandoNaoHaNotificacoes()
        {
            // Arrange
            var curso = new Curso("Curso Teste", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var turma = new Turma("Turma A", curso.Id, "Ativa");
            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetNotificacoesByTurmaId(turma.Id);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
