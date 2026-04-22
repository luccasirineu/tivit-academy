using FluentAssertions;
using tivitApi.Enums;
using tivitApi.Models;

namespace tivitApi.Tests.Models
{
    public class MatriculaTests
    {
        [Fact]
        public void Matricula_DeveCriarInstanciaComConstrutorSimples()
        {
            // Arrange & Act
            var matricula = new Matricula("João Silva", "joao@test.com", "12345678900", 1);

            // Assert
            matricula.Nome.Should().Be("João Silva");
            matricula.Email.Should().Be("joao@test.com");
            matricula.Cpf.Should().Be("12345678900");
            matricula.CursoId.Should().Be(1);
            matricula.Status.Should().Be(StatusMatricula.AGUARDANDO_PAGAMENTO);
        }

        [Fact]
        public void Matricula_DeveCriarInstanciaComConstrutorCompleto()
        {
            // Arrange & Act
            var matricula = new Matricula("Maria Santos", "maria@test.com", "98765432100", StatusMatricula.APROVADO, 2);

            // Assert
            matricula.Nome.Should().Be("Maria Santos");
            matricula.Email.Should().Be("maria@test.com");
            matricula.Cpf.Should().Be("98765432100");
            matricula.Status.Should().Be(StatusMatricula.APROVADO);
            matricula.CursoId.Should().Be(2);
        }

        [Fact]
        public void Matricula_DeveCriarInstanciaComConstrutorVazio()
        {
            // Arrange & Act
            var matricula = new Matricula();

            // Assert
            matricula.Should().NotBeNull();
        }

        [Fact]
        public void Matricula_DeveInicializarComStatusAguardandoPagamentoPorPadrao()
        {
            // Arrange & Act
            var matricula = new Matricula("Teste", "teste@test.com", "12345678900", 1);

            // Assert
            matricula.Status.Should().Be(StatusMatricula.AGUARDANDO_PAGAMENTO);
        }

        [Theory]
        [InlineData(StatusMatricula.AGUARDANDO_PAGAMENTO)]
        [InlineData(StatusMatricula.AGUARDANDO_DOCUMENTOS)]
        [InlineData(StatusMatricula.AGUARDANDO_APROVACAO)]
        [InlineData(StatusMatricula.APROVADO)]
        [InlineData(StatusMatricula.RECUSADO)]
        public void Matricula_DeveAceitarTodosOsStatusPossiveis(StatusMatricula status)
        {
            // Arrange & Act
            var matricula = new Matricula("Aluno", "aluno@test.com", "12345678900", status, 1);

            // Assert
            matricula.Status.Should().Be(status);
        }

        [Fact]
        public void Matricula_DevePermitirAlteracaoDeStatus()
        {
            // Arrange
            var matricula = new Matricula("Aluno", "aluno@test.com", "12345678900", 1);

            // Act
            matricula.Status = StatusMatricula.APROVADO;

            // Assert
            matricula.Status.Should().Be(StatusMatricula.APROVADO);
        }

        [Fact]
        public void Matricula_DeveManterRelacionamentoComCurso()
        {
            // Arrange
            var curso = new Curso("Medicina", "Curso de Medicina", 1, StatusCurso.ATIVO);

            // Act
            var matricula = new Matricula("Aluno", "aluno@test.com", "12345678900", curso.Id)
            {
                curso = curso
            };

            // Assert
            matricula.curso.Should().NotBeNull();
            matricula.curso.Should().Be(curso);
            matricula.CursoId.Should().Be(curso.Id);
        }
    }
}
