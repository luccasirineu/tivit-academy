using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Enums;
using tivitApi.Infra.SQS;
using tivitApi.Models;
using tivitApi.Services;
using tivitApi.Tests.Helpers;

namespace tivitApi.Tests.Services
{
    public class ProfessorServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly Mock<ISQSProducer > _mockSQSProducer;
        private readonly Mock<ILogger<MatriculaService>> _mockLogger;
        private readonly ProfessorService _service;

        public ProfessorServiceTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockSQSProducer = new Mock<ISQSProducer >();
            _mockLogger = new Mock<ILogger<MatriculaService>>();

            _service = new ProfessorService(
                _context,
                _mockPasswordHasher.Object,
                _mockSQSProducer.Object,
                _mockLogger.Object
            );
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetQntdProfessoresAtivos_DeveRetornarQuantidadeCorreta()
        {
            // Arrange
            var professor1 = new Professor("Maria Santos", "maria@email.com", "RM123456", "98765432100", StatusUsuario.ATIVO);
            var professor2 = new Professor("João Silva", "joao@email.com", "RM654321", "12345678900", StatusUsuario.ATIVO);
            _context.Professores.AddRange(professor1, professor2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetQntdProfessoresAtivos();

            // Assert
            result.Should().Be(2);
        }

        [Fact]
        public async Task GetProfessorById_DeveRetornarProfessor_QuandoExiste()
        {
            // Arrange
            var professor = new Professor("Maria Santos", "maria@email.com", "RM123456", "98765432100", StatusUsuario.ATIVO);
            _context.Professores.Add(professor);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetProfessorById(professor.Id);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be("Maria Santos");
            result.Rm.Should().Be("RM123456");
        }

        [Fact]
        public async Task GetAllProfessores_DeveRetornarTodosProfessores()
        {
            // Arrange
            var professor1 = new Professor("Maria Santos", "maria@email.com", "RM123456", "98765432100", StatusUsuario.ATIVO);
            var professor2 = new Professor("João Silva", "joao@email.com", "RM654321", "12345678900", StatusUsuario.ATIVO);
            _context.Professores.AddRange(professor1, professor2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllProfessores();

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllProfessoresAtivos_DeveRetornarApenasProfessoresAtivos()
        {
            // Arrange
            var professor1 = new Professor("Maria Santos", "maria@email.com", "RM123456", "98765432100", StatusUsuario.ATIVO);
            var professor2 = new Professor("João Silva", "joao@email.com", "RM654321", "12345678900", StatusUsuario.ATIVO);
            professor2.Status = StatusUsuario.DESATIVADO;
            _context.Professores.AddRange(professor1, professor2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllProfessoresAtivos();

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(1);
            result[0].Nome.Should().Be("Maria Santos");
        }

        [Fact]
        public async Task GerarRm_DeveGerarRmUnico()
        {
            // Arrange
            var professor = new Professor("Maria Santos", "maria@email.com", "RM123456", "98765432100", StatusUsuario.ATIVO);
            _context.Professores.Add(professor);
            await _context.SaveChangesAsync();

            // Act
            var rm = await _service.GerarRm();

            // Assert
            rm.Should().NotBeNullOrEmpty();
            rm.Should().StartWith("RM");
            rm.Length.Should().Be(8); // RM + 6 dígitos
            rm.Should().NotBe("RM123456"); // Deve ser diferente do existente
        }

        [Fact]
        public async Task CriarProfessor_DeveCriarProfessorComSucesso()
        {
            // Arrange
            var dto = new ProfessorDTORequest
            {
                Nome = "Maria Santos",
                Email = "maria@email.com",
                Cpf = "98765432100"
            };

            _mockPasswordHasher.Setup(p => p.Hash(It.IsAny<string>())).Returns("senhaHash");
            _mockSQSProducer.Setup(s => s.EnviarEventoAsync(It.IsAny<ProfessorStatusEvento>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.CriarProfessor(dto);

            // Assert
            var professores = _context.Professores.ToList();
            professores.Should().HaveCount(1);
            professores[0].Nome.Should().Be("Maria Santos");
            professores[0].Email.Should().Be("maria@email.com");
            professores[0].Rm.Should().StartWith("RM");
        }
    }
}
