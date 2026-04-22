using FluentAssertions;
using tivitApi.Enums;
using tivitApi.Models;

namespace tivitApi.Tests.Models
{
    public class CursoTests
    {
        [Fact]
        public void Curso_DeveCriarInstanciaComConstrutorCompleto()
        {
            // Arrange & Act
            var curso = new Curso("Engenharia", "Curso de Engenharia", 1, StatusCurso.ATIVO);

            // Assert
            curso.Nome.Should().Be("Engenharia");
            curso.Descricao.Should().Be("Curso de Engenharia");
            curso.ProfResponsavel.Should().Be(1);
            curso.Status.Should().Be(StatusCurso.ATIVO);
        }

        [Theory]
        [InlineData(StatusCurso.ATIVO)]
        [InlineData(StatusCurso.DESATIVADO)]
        public void Curso_DeveAceitarTodosOsStatusPossiveis(StatusCurso status)
        {
            // Arrange & Act
            var curso = new Curso("Medicina", "Curso de Medicina", 1, status);

            // Assert
            curso.Status.Should().Be(status);
        }

        [Fact]
        public void Curso_DevePermitirAlteracaoDeStatus()
        {
            // Arrange
            var curso = new Curso("Direito", "Curso de Direito", 1, StatusCurso.ATIVO);

            // Act
            curso.Status = StatusCurso.DESATIVADO;

            // Assert
            curso.Status.Should().Be(StatusCurso.DESATIVADO);
        }

        [Fact]
        public void Curso_DeveManterRelacionamentoComProfessor()
        {
            // Arrange
            var professor = new Professor("Prof. João", "prof@test.com", "RM123", "12345678900", StatusUsuario.ATIVO);
            
            // Act
            var curso = new Curso("Matemática", "Curso de Matemática", professor.Id, StatusCurso.ATIVO)
            {
                Professor = professor
            };

            // Assert
            curso.Professor.Should().NotBeNull();
            curso.Professor.Should().Be(professor);
            curso.ProfResponsavel.Should().Be(professor.Id);
        }

        [Fact]
        public void Curso_DevePermitirAlteracaoDeDescricao()
        {
            // Arrange
            var curso = new Curso("Física", "Descrição antiga", 1, StatusCurso.ATIVO);

            // Act
            curso.Descricao = "Nova descrição do curso";

            // Assert
            curso.Descricao.Should().Be("Nova descrição do curso");
        }
    }
}
