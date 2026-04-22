using FluentAssertions;
using tivitApi.Enums;
using tivitApi.Models;

namespace tivitApi.Tests.Models
{
    public class ProfessorTests
    {
        [Fact]
        public void Professor_DeveCriarInstanciaComConstrutorCompleto()
        {
            // Arrange & Act
            var professor = new Professor("Prof. João", "prof@test.com", "RM12345", "12345678900", StatusUsuario.ATIVO);

            // Assert
            professor.Nome.Should().Be("Prof. João");
            professor.Email.Should().Be("prof@test.com");
            professor.Rm.Should().Be("RM12345");
            professor.Cpf.Should().Be("12345678900");
            professor.Status.Should().Be(StatusUsuario.ATIVO);
        }

        [Fact]
        public void Professor_DeveCriarInstanciaComConstrutorVazio()
        {
            // Arrange & Act
            var professor = new Professor();

            // Assert
            professor.Should().NotBeNull();
        }

        [Theory]
        [InlineData(StatusUsuario.ATIVO)]
        [InlineData(StatusUsuario.DESATIVADO)]
        [InlineData(StatusUsuario.BLOQUEADO)]
        public void Professor_DeveAceitarTodosOsStatusPossiveis(StatusUsuario status)
        {
            // Arrange & Act
            var professor = new Professor("Prof. Maria", "maria@test.com", "RM99999", "98765432100", status);

            // Assert
            professor.Status.Should().Be(status);
        }

        [Fact]
        public void Professor_DevePermitirDefinirSenha()
        {
            // Arrange
            var professor = new Professor("Prof. Carlos", "carlos@test.com", "RM11111", "11111111111", StatusUsuario.ATIVO);

            // Act
            professor.Senha = "senha123";

            // Assert
            professor.Senha.Should().Be("senha123");
        }

        [Fact]
        public void Professor_DevePermitirAlteracaoDeStatus()
        {
            // Arrange
            var professor = new Professor("Prof. Ana", "ana@test.com", "RM22222", "22222222222", StatusUsuario.ATIVO);

            // Act
            professor.Status = StatusUsuario.DESATIVADO;

            // Assert
            professor.Status.Should().Be(StatusUsuario.DESATIVADO);
        }

        [Fact]
        public void Professor_DeveArmazenarRmCorretamente()
        {
            // Arrange & Act
            var professor = new Professor("Prof. Pedro", "pedro@test.com", "RM54321", "33333333333", StatusUsuario.ATIVO);

            // Assert
            professor.Rm.Should().Be("RM54321");
        }

        [Fact]
        public void Professor_DevePermitirAlteracaoDeEmail()
        {
            // Arrange
            var professor = new Professor("Prof. Lucia", "lucia@test.com", "RM77777", "44444444444", StatusUsuario.ATIVO);

            // Act
            professor.Email = "novoemail@test.com";

            // Assert
            professor.Email.Should().Be("novoemail@test.com");
        }
    }
}
