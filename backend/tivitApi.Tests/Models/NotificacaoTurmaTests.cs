using FluentAssertions;
using tivitApi.Enums;
using tivitApi.Models;

namespace tivitApi.Tests.Models
{
    public class NotificacaoTurmaTests
    {
        [Fact]
        public void NotificacaoTurma_DeveCriarInstanciaComPropriedadesCorretas()
        {
            // Arrange & Act
            var notificacaoTurma = new NotificacaoTurma
            {
                NotificacaoId = 1,
                TurmaId = 2
            };

            // Assert
            notificacaoTurma.NotificacaoId.Should().Be(1);
            notificacaoTurma.TurmaId.Should().Be(2);
        }

        [Fact]
        public void NotificacaoTurma_DeveManterRelacionamentoComNotificacao()
        {
            // Arrange
            var notificacao = new Notificacao("Aviso", "Descrição");

            // Act
            var notificacaoTurma = new NotificacaoTurma
            {
                NotificacaoId = notificacao.Id,
                Notificacao = notificacao
            };

            // Assert
            notificacaoTurma.Notificacao.Should().NotBeNull();
            notificacaoTurma.Notificacao.Should().Be(notificacao);
        }

        [Fact]
        public void NotificacaoTurma_DeveManterRelacionamentoComTurma()
        {
            // Arrange
            var turma = new Turma("Turma A", 1, "ATIVO");

            // Act
            var notificacaoTurma = new NotificacaoTurma
            {
                TurmaId = turma.Id,
                Turma = turma
            };

            // Assert
            notificacaoTurma.Turma.Should().NotBeNull();
            notificacaoTurma.Turma.Should().Be(turma);
        }

        [Fact]
        public void NotificacaoTurma_DeveManterAmbosRelacionamentos()
        {
            // Arrange
            var notificacao = new Notificacao("Título", "Descrição");
            var turma = new Turma("Turma B", 1, "ATIVO");

            // Act
            var notificacaoTurma = new NotificacaoTurma
            {
                NotificacaoId = notificacao.Id,
                Notificacao = notificacao,
                TurmaId = turma.Id,
                Turma = turma
            };

            // Assert
            notificacaoTurma.Notificacao.Should().Be(notificacao);
            notificacaoTurma.Turma.Should().Be(turma);
            notificacaoTurma.NotificacaoId.Should().Be(notificacao.Id);
            notificacaoTurma.TurmaId.Should().Be(turma.Id);
        }
    }
}
