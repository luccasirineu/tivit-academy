using FluentAssertions;
using tivitApi.Enums;
using tivitApi.Models;

namespace tivitApi.Tests.Models
{
    public class AlunoTests
    {
        [Fact]
        public void Aluno_DeveCriarInstanciaComConstrutorCompleto()
        {
            // Arrange & Act
            var aluno = new Aluno("João Silva", "joao@test.com", "12345678900", "senha123", 1);

            // Assert
            aluno.Nome.Should().Be("João Silva");
            aluno.Email.Should().Be("joao@test.com");
            aluno.Cpf.Should().Be("12345678900");
            aluno.Senha.Should().Be("senha123");
            aluno.MatriculaId.Should().Be(1);
        }

        [Fact]
        public void Aluno_DeveCriarInstanciaComConstrutorVazio()
        {
            // Arrange & Act
            var aluno = new Aluno();

            // Assert
            aluno.Should().NotBeNull();
        }

        [Fact]
        public void Aluno_DevePermitirDefinirTurmaId()
        {
            // Arrange
            var aluno = new Aluno("João", "joao@test.com", "12345678900", "senha", 1);

            // Act
            aluno.TurmaId = 5;

            // Assert
            aluno.TurmaId.Should().Be(5);
        }

        [Theory]
        [InlineData(StatusUsuario.ATIVO)]
        [InlineData(StatusUsuario.DESATIVADO)]
        [InlineData(StatusUsuario.BLOQUEADO)]
        public void Aluno_DeveAceitarTodosOsStatusPossiveis(StatusUsuario status)
        {
            // Arrange & Act
            var aluno = new Aluno
            {
                Nome = "Aluno",
                Email = "aluno@test.com",
                Cpf = "12345678900",
                Senha = "senha",
                MatriculaId = 1,
                Status = status
            };

            // Assert
            aluno.Status.Should().Be(status);
        }

        [Fact]
        public void Aluno_DeveManterRelacionamentoComMatricula()
        {
            // Arrange
            var matricula = new Matricula("João", "joao@test.com", "12345678900", 1);
            var aluno = new Aluno("João", "joao@test.com", "12345678900", "senha", matricula.Id)
            {
                Matricula = matricula
            };

            // Assert
            aluno.Matricula.Should().NotBeNull();
            aluno.Matricula.Should().Be(matricula);
            aluno.MatriculaId.Should().Be(matricula.Id);
        }
    }
}
