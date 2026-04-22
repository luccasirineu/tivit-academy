using FluentAssertions;
using tivitApi.Models;

namespace tivitApi.Tests.Models
{
    public class NotificacaoTests
    {
        [Fact]
        public void Notificacao_DeveCriarInstanciaComConstrutorCompleto()
        {
            // Arrange & Act
            var notificacao = new Notificacao("Aviso Importante", "Descrição do aviso");

            // Assert
            notificacao.Titulo.Should().Be("Aviso Importante");
            notificacao.Descricao.Should().Be("Descrição do aviso");
        }

        [Fact]
        public void Notificacao_DeveInicializarDataCriacaoAutomaticamente()
        {
            // Arrange
            var antes = DateTime.UtcNow;

            // Act
            var notificacao = new Notificacao("Título", "Descrição");
            var depois = DateTime.UtcNow;

            // Assert
            notificacao.DataCriacao.Should().BeOnOrAfter(antes);
            notificacao.DataCriacao.Should().BeOnOrBefore(depois);
        }

        [Fact]
        public void Notificacao_DevePermitirColecaoDeNotificacaoTurmas()
        {
            // Arrange
            var notificacao = new Notificacao("Título", "Descrição");
            var turma1 = new Turma("Turma A", 1, "ATIVO");
            var turma2 = new Turma("Turma B", 1, "ATIVO");

            var notificacaoTurmas = new List<NotificacaoTurma>
            {
                new NotificacaoTurma { NotificacaoId = notificacao.Id, Notificacao = notificacao, TurmaId = turma1.Id, Turma = turma1 },
                new NotificacaoTurma { NotificacaoId = notificacao.Id, Notificacao = notificacao, TurmaId = turma2.Id, Turma = turma2 }
            };

            // Act
            notificacao.NotificacaoTurmas = notificacaoTurmas;

            // Assert
            notificacao.NotificacaoTurmas.Should().HaveCount(2);
        }

        [Fact]
        public void Notificacao_DevePermitirAlteracaoDePropriedades()
        {
            // Arrange
            var notificacao = new Notificacao("Título Original", "Descrição Original");

            // Act
            notificacao.Titulo = "Novo Título";
            notificacao.Descricao = "Nova Descrição";

            // Assert
            notificacao.Titulo.Should().Be("Novo Título");
            notificacao.Descricao.Should().Be("Nova Descrição");
        }

        [Fact]
        public void Notificacao_DeveAceitarDescricaoLonga()
        {
            // Arrange
            var descricaoLonga = new string('A', 1000);

            // Act
            var notificacao = new Notificacao("Título", descricaoLonga);

            // Assert
            notificacao.Descricao.Should().HaveLength(1000);
        }
    }
}
