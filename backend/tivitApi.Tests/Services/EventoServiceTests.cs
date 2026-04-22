using FluentAssertions;
using Microsoft.Extensions.Logging;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Exceptions;
using tivitApi.Models;
using tivitApi.Services;
using tivitApi.Tests.Helpers;

namespace tivitApi.Tests.Services
{
    public class EventoServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly EventoService _service;
        private readonly ILogger<EventoService> _logger;

        public EventoServiceTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _logger = new LoggerFactory().CreateLogger<EventoService>();
            _service = new EventoService(_context, _logger);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task CriarEvento_DeveCriarEventoComSucesso()
        {
            // Arrange
            var dto = new EventoDTO
            {
                Titulo = "Evento Teste",
                Descricao = "Descrição do evento",
                Horario = DateTime.Now.AddDays(1)
            };

            // Act
            var result = await _service.criarEvento(dto);

            // Assert
            result.Should().NotBeNull();
            int eventoId = ((dynamic)result).eventoId;
            eventoId.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CriarEvento_DeveLancarExcecao_QuandoTituloVazio()
        {
            // Arrange
            var dto = new EventoDTO
            {
                Titulo = "",
                Descricao = "Descrição",
                Horario = DateTime.Now.AddDays(1)
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _service.criarEvento(dto));
        }

        [Fact]
        public async Task ObterProximoEvento_DeveRetornarProximoEvento()
        {
            // Arrange
            var evento1 = new Evento("Evento 1", "Descrição 1", DateTime.Now.AddDays(1));
            var evento2 = new Evento("Evento 2", "Descrição 2", DateTime.Now.AddDays(2));

            _context.Eventos.AddRange(evento1, evento2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.obterProximoEvento();

            // Assert
            result.Should().NotBeNull();
            result.Titulo.Should().Be("Evento 1");
        }

        [Fact]
        public async Task ObterProximoEvento_DeveLancarExcecao_QuandoNaoHaEventosFuturos()
        {
            // Arrange
            var evento = new Evento("Evento Passado", "Descrição", DateTime.Now.AddDays(-1));
            _context.Eventos.Add(evento);
            await _context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.obterProximoEvento());
        }

        [Fact]
        public async Task GetAllEvents_DeveRetornarTodosEventos()
        {
            // Arrange
            var evento1 = new Evento("Evento 1", "Descrição 1", DateTime.Now.AddDays(1));
            var evento2 = new Evento("Evento 2", "Descrição 2", DateTime.Now.AddDays(2));

            _context.Eventos.AddRange(evento1, evento2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.getAllEvents();

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetNextWeekEvents_DeveRetornarQuantidadeDeEventosDaProximaSemana()
        {
            // Arrange
            var evento1 = new Evento("Evento 1", "Descrição 1", DateTime.Now.AddDays(1));
            var evento2 = new Evento("Evento 2", "Descrição 2", DateTime.Now.AddDays(3));
            var evento3 = new Evento("Evento 3", "Descrição 3", DateTime.Now.AddDays(10));

            _context.Eventos.AddRange(evento1, evento2, evento3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.getNextWeekEvents();

            // Assert
            result.Should().Be(2);
        }
    }
}
