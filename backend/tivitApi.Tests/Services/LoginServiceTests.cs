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
    public class LoginServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly Mock<ILogger<LoginService>> _mockLogger;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly LoginService _service;

        public LoginServiceTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _mockLogger = new Mock<ILogger<LoginService>>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockTokenService = new Mock<ITokenService>();

            _service = new LoginService(
                _context,
                _mockLogger.Object,
                _mockPasswordHasher.Object,
                _mockTokenService.Object
            );
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task LoginAsync_DeveAutenticarAluno_QuandoCredenciaisValidas()
        {
            // Arrange
            var curso = new Curso("Curso Teste", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var matricula = new Matricula("João Silva", "joao@email.com", "12345678900", curso.Id);
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            var aluno = new Aluno("João Silva", "joao@email.com", "12345678900", "senhaHash", matricula.Id);
            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            var loginDTO = new LoginDTO
            {
                Cpf = "12345678900",
                Senha = "senha123",
                Tipo = "aluno"
            };

            _mockPasswordHasher.Setup(p => p.Verificar("senha123", "senhaHash")).Returns(true);
            _mockTokenService.Setup(t => t.GerarToken(It.IsAny<LoginDTOResponse>())).Returns("token123");

            // Act
            var result = await _service.LoginAsync(loginDTO);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be("João Silva");
            result.Tipo.Should().Be("aluno");
            result.Token.Should().Be("token123");
        }

        [Fact]
        public async Task LoginAsync_DeveAutenticarProfessor_QuandoCredenciaisValidas()
        {
            // Arrange
            var professor = new Professor("Maria Santos", "maria@email.com", "RM123456", "98765432100", StatusUsuario.ATIVO)
            {
                Senha = "senhaHash"
            };
            _context.Professores.Add(professor);
            await _context.SaveChangesAsync();

            var loginDTO = new LoginDTO
            {
                Cpf = "98765432100",
                Senha = "senha123",
                Tipo = "professor"
            };

            _mockPasswordHasher.Setup(p => p.Verificar("senha123", "senhaHash")).Returns(true);
            _mockTokenService.Setup(t => t.GerarToken(It.IsAny<LoginDTOResponse>())).Returns("token123");

            // Act
            var result = await _service.LoginAsync(loginDTO);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be("Maria Santos");
            result.Tipo.Should().Be("professor");
            result.Token.Should().Be("token123");
        }

        [Fact]
        public async Task LoginAsync_DeveAutenticarAdministrador_QuandoCredenciaisValidas()
        {
            // Arrange
            var admin = new Administrador
            {
                Nome = "Admin User",
                Email = "admin@email.com",
                Cpf = "11111111111",
                Senha = "senhaHash",
                Status = StatusUsuario.ATIVO
            };
            _context.Administradores.Add(admin);
            await _context.SaveChangesAsync();

            var loginDTO = new LoginDTO
            {
                Cpf = "11111111111",
                Senha = "senha123",
                Tipo = "administrador"
            };

            _mockTokenService.Setup(t => t.GerarToken(It.IsAny<LoginDTOResponse>())).Returns("token123");

            // Act
            var result = await _service.LoginAsync(loginDTO);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be("Admin User");
            result.Tipo.Should().Be("administrador");
            result.Token.Should().Be("token123");
        }

        [Fact]
        public async Task LoginAsync_DeveLancarExcecao_QuandoSenhaInvalida()
        {
            // Arrange
            var curso = new Curso("Curso Teste", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var matricula = new Matricula("João Silva", "joao@email.com", "12345678900", curso.Id);
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            var aluno = new Aluno("João Silva", "joao@email.com", "12345678900", "senhaHash", matricula.Id);
            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            var loginDTO = new LoginDTO
            {
                Cpf = "12345678900",
                Senha = "senhaErrada",
                Tipo = "aluno"
            };

            _mockPasswordHasher.Setup(p => p.Verificar("senhaErrada", "senhaHash")).Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<CredenciaisInvalidasException>(() => _service.LoginAsync(loginDTO));
        }

        [Fact]
        public async Task LoginAsync_DeveLancarExcecao_QuandoUsuarioNaoExiste()
        {
            // Arrange
            var loginDTO = new LoginDTO
            {
                Cpf = "00000000000",
                Senha = "senha123",
                Tipo = "aluno"
            };

            // Act & Assert
            await Assert.ThrowsAsync<CredenciaisInvalidasException>(() => _service.LoginAsync(loginDTO));
        }

        [Fact]
        public async Task LoginAsync_DeveLancarExcecao_QuandoTipoInvalido()
        {
            // Arrange
            var loginDTO = new LoginDTO
            {
                Cpf = "12345678900",
                Senha = "senha123",
                Tipo = "invalido"
            };

            // Act & Assert
            await Assert.ThrowsAsync<RequisicaoInvalidaException>(() => _service.LoginAsync(loginDTO));
        }

        [Fact]
        public async Task LoginAsync_DeveLancarExcecao_QuandoRequisicaoNula()
        {
            // Act & Assert
            await Assert.ThrowsAsync<RequisicaoInvalidaException>(() => _service.LoginAsync(null));
        }

        [Fact]
        public async Task LoginAsync_DeveLancarExcecao_QuandoUsuarioDesativado()
        {
            // Arrange
            var curso = new Curso("Curso Teste", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var matricula = new Matricula("João Silva", "joao@email.com", "12345678900", curso.Id);
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            var aluno = new Aluno("João Silva", "joao@email.com", "12345678900", "senhaHash", matricula.Id);
            aluno.Status = StatusUsuario.DESATIVADO;
            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            var loginDTO = new LoginDTO
            {
                Cpf = "12345678900",
                Senha = "senha123",
                Tipo = "aluno"
            };

            // Act & Assert
            await Assert.ThrowsAsync<CredenciaisInvalidasException>(() => _service.LoginAsync(loginDTO));
        }
    }
}
