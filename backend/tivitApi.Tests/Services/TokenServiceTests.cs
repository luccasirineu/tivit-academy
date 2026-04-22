using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using tivitApi.DTOs;
using tivitApi.Services;

namespace tivitApi.Tests.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly TokenService _service;

        public TokenServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            
            // Configurar valores de configuração mockados
            _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("ChaveSecretaSuperSeguraComMaisDe32Caracteres123456");
            _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("TivitApi");
            _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("TivitApiUsers");
            _mockConfiguration.Setup(c => c["Jwt:ExpiresInHours"]).Returns("24");

            _service = new TokenService(_mockConfiguration.Object);
        }

        [Fact]
        public void GerarToken_DeveGerarTokenValido_ParaAluno()
        {
            // Arrange
            var usuario = new LoginDTOResponse
            {
                Id = 1,
                Nome = "João Silva",
                Tipo = "aluno",
                Cpf = "12345678900",
                TurmaId = 1,
                CursosIds = new List<int> { 1 }
            };

            // Act
            var token = _service.GerarToken(usuario);

            // Assert
            token.Should().NotBeNullOrEmpty();
            token.Split('.').Should().HaveCount(3); // JWT tem 3 partes separadas por ponto
        }

        [Fact]
        public void GerarToken_DeveGerarTokenValido_ParaProfessor()
        {
            // Arrange
            var usuario = new LoginDTOResponse
            {
                Id = 2,
                Nome = "Maria Santos",
                Tipo = "professor",
                Cpf = "98765432100",
                TurmaId = 0,
                CursosIds = new List<int> { 2 }
            };

            // Act
            var token = _service.GerarToken(usuario);

            // Assert
            token.Should().NotBeNullOrEmpty();
            token.Split('.').Should().HaveCount(3);
        }

        [Fact]
        public void GerarToken_DeveGerarTokenValido_ParaAdministrador()
        {
            // Arrange
            var usuario = new LoginDTOResponse
            {
                Id = 3,
                Nome = "Admin User",
                Tipo = "administrador",
                Cpf = "11111111111",
                TurmaId = 0,
                CursosIds = new List<int> { 3 }
            };

            // Act
            var token = _service.GerarToken(usuario);

            // Assert
            token.Should().NotBeNullOrEmpty();
            token.Split('.').Should().HaveCount(3);
        }

        [Fact]
        public void GerarToken_DeveGerarTokensDiferentes_ParaUsuariosDiferentes()
        {
            // Arrange
            var usuario1 = new LoginDTOResponse
            {
                Id = 1,
                Nome = "João Silva",
                Tipo = "aluno",
                Cpf = "12345678900",
                TurmaId = 1,
                CursosIds = new List<int> { 1 }
            };

            var usuario2 = new LoginDTOResponse
            {
                Id = 2,
                Nome = "Maria Santos",
                Tipo = "professor",
                Cpf = "98765432100",
                TurmaId = 0,
                CursosIds = new List<int> { 2 }
            };

            // Act
            var token1 = _service.GerarToken(usuario1);
            var token2 = _service.GerarToken(usuario2);

            // Assert
            token1.Should().NotBe(token2);
        }
    }
}
