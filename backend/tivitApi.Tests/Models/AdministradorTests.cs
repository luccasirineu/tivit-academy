using FluentAssertions;
using tivitApi.Enums;
using tivitApi.Models;

namespace tivitApi.Tests.Models
{
    public class AdministradorTests
    {
        [Fact]
        public void Administrador_DeveCriarInstanciaComPropriedadesCorretas()
        {
            // Arrange & Act
            var admin = new Administrador
            {
                Id = 1,
                Nome = "Admin Teste",
                Email = "admin@test.com",
                Cpf = "12345678900",
                Senha = "senha123",
                Status = StatusUsuario.ATIVO
            };

            // Assert
            admin.Id.Should().Be(1);
            admin.Nome.Should().Be("Admin Teste");
            admin.Email.Should().Be("admin@test.com");
            admin.Cpf.Should().Be("12345678900");
            admin.Senha.Should().Be("senha123");
            admin.Status.Should().Be(StatusUsuario.ATIVO);
        }

        [Theory]
        [InlineData(StatusUsuario.ATIVO)]
        [InlineData(StatusUsuario.DESATIVADO)]
        [InlineData(StatusUsuario.BLOQUEADO)]
        public void Administrador_DeveAceitarTodosOsStatusPossiveis(StatusUsuario status)
        {
            // Arrange & Act
            var admin = new Administrador
            {
                Nome = "Admin",
                Email = "admin@test.com",
                Cpf = "12345678900",
                Senha = "senha",
                Status = status
            };

            // Assert
            admin.Status.Should().Be(status);
        }

        [Fact]
        public void Administrador_DevePermitirAlteracaoDeSenha()
        {
            // Arrange
            var admin = new Administrador
            {
                Nome = "Admin",
                Email = "admin@test.com",
                Cpf = "12345678900",
                Senha = "senhaAntiga",
                Status = StatusUsuario.ATIVO
            };

            // Act
            admin.Senha = "novaSenha";

            // Assert
            admin.Senha.Should().Be("novaSenha");
        }
    }
}
