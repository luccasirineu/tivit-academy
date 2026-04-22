using FluentAssertions;
using tivitApi.Models;

namespace tivitApi.Tests.Models
{
    public class EventoTests
    {
        [Fact]
        public void Evento_DeveCriarInstanciaComConstrutorCompleto()
        {
            // Arrange
            var horario = DateTime.UtcNow;

            // Act
            var evento = new Evento("Palestra", "Palestra sobre IA", horario);

            // Assert
            evento.Titulo.Should().Be("Palestra");
            evento.Descricao.Should().Be("Palestra sobre IA");
            evento.Horario.Should().Be(horario);
        }

        [Fact]
        public void Evento_DeveCriarInstanciaComConstrutorVazio()
        {
            // Arrange & Act
            var evento = new Evento();

            // Assert
            evento.Should().NotBeNull();
        }

        [Fact]
        public void Evento_DeveArmazenarHorarioCorretamente()
        {
            // Arrange
            var horario = new DateTime(2024, 12, 25, 14, 30, 0);

            // Act
            var evento = new Evento("Evento Teste", "Descrição", horario);

            // Assert
            evento.Horario.Should().Be(horario);
            evento.Horario.Year.Should().Be(2024);
            evento.Horario.Month.Should().Be(12);
            evento.Horario.Day.Should().Be(25);
        }

        [Fact]
        public void Evento_DevePermitirAlteracaoDePropriedades()
        {
            // Arrange
            var evento = new Evento("Título Original", "Descrição Original", DateTime.UtcNow);

            // Act
            evento.Titulo = "Novo Título";
            evento.Descricao = "Nova Descrição";
            var novoHorario = DateTime.UtcNow.AddDays(1);
            evento.Horario = novoHorario;

            // Assert
            evento.Titulo.Should().Be("Novo Título");
            evento.Descricao.Should().Be("Nova Descrição");
            evento.Horario.Should().Be(novoHorario);
        }

        [Fact]
        public void Evento_DeveAceitarDescricaoLonga()
        {
            // Arrange
            var descricaoLonga = new string('A', 500);
            var horario = DateTime.UtcNow;

            // Act
            var evento = new Evento("Evento", descricaoLonga, horario);

            // Assert
            evento.Descricao.Should().HaveLength(500);
        }
    }
}
