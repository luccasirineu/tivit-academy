using FluentAssertions;
using tivitApi.Enums;
using tivitApi.Models;

namespace tivitApi.Tests.Models
{
    public class TurmaTests
    {
        [Fact]
        public void Turma_DeveCriarInstanciaComConstrutorCompleto()
        {
            // Arrange & Act
            var turma = new Turma("Turma A", 1, "ATIVO");

            // Assert
            turma.Nome.Should().Be("Turma A");
            turma.CursoId.Should().Be(1);
            turma.Status.Should().Be("ATIVO");
        }

        [Theory]
        [InlineData("ATIVO")]
        [InlineData("INATIVO")]
        public void Turma_DeveAceitarDiferentesStatus(string status)
        {
            // Arrange & Act
            var turma = new Turma("Turma B", 1, status);

            // Assert
            turma.Status.Should().Be(status);
        }

        [Fact]
        public void Turma_DevePermitirAlteracaoDeStatus()
        {
            // Arrange
            var turma = new Turma("Turma C", 1, "ATIVO");

            // Act
            turma.Status = "INATIVO";

            // Assert
            turma.Status.Should().Be("INATIVO");
        }

        [Fact]
        public void Turma_DeveManterRelacionamentoComCurso()
        {
            // Arrange
            var curso = new Curso("Engenharia", "Curso de Engenharia", 1, StatusCurso.ATIVO);

            // Act
            var turma = new Turma("Turma D", curso.Id, "ATIVO")
            {
                Curso = curso
            };

            // Assert
            turma.Curso.Should().NotBeNull();
            turma.Curso.Should().Be(curso);
            turma.CursoId.Should().Be(curso.Id);
        }

        [Fact]
        public void Turma_DevePermitirColecaoDeNotificacaoTurmas()
        {
            // Arrange
            var turma = new Turma("Turma E", 1, "ATIVO");
            var notificacao1 = new Notificacao("Aviso 1", "Descrição 1");
            var notificacao2 = new Notificacao("Aviso 2", "Descrição 2");

            var notificacaoTurmas = new List<NotificacaoTurma>
            {
                new NotificacaoTurma { NotificacaoId = notificacao1.Id, Notificacao = notificacao1, TurmaId = turma.Id, Turma = turma },
                new NotificacaoTurma { NotificacaoId = notificacao2.Id, Notificacao = notificacao2, TurmaId = turma.Id, Turma = turma }
            };

            // Act
            turma.NotificacaoTurmas = notificacaoTurmas;

            // Assert
            turma.NotificacaoTurmas.Should().HaveCount(2);
        }

        [Fact]
        public void Turma_DevePermitirAlteracaoDeNome()
        {
            // Arrange
            var turma = new Turma("Nome Original", 1, "ATIVO");

            // Act
            turma.Nome = "Novo Nome";

            // Assert
            turma.Nome.Should().Be("Novo Nome");
        }
    }
}
