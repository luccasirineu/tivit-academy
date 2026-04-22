using FluentAssertions;
using tivitApi.Mappers;
using tivitApi.Models;

namespace tivitApi.Tests.Mappers
{
    public class EventoMapperTests
    {
        [Fact]
        public void EventoMapper_ToDTO_DeveConverterCorretamente()
        {
            // Arrange
            var horario = new DateTime(2024, 12, 25, 14, 30, 0);
            var evento = new Evento("Palestra de Tecnologia", "Palestra sobre IA e Machine Learning", horario)
            {
                Id = 1
            };

            // Act
            var dto = evento.ToDTO();

            // Assert
            dto.Should().NotBeNull();
            dto.Id.Should().Be(1);
            dto.Titulo.Should().Be("Palestra de Tecnologia");
            dto.Descricao.Should().Be("Palestra sobre IA e Machine Learning");
            dto.Horario.Should().Be(horario);
        }

        [Fact]
        public void EventoMapper_ToDTO_DeveConverterEventoComDescricaoLonga()
        {
            // Arrange
            var descricaoLonga = "Esta é uma descrição muito longa que contém muitos detalhes sobre o evento, " +
                                "incluindo informações sobre palestrantes, tópicos a serem abordados e muito mais.";
            var evento = new Evento("Workshop", descricaoLonga, DateTime.Now)
            {
                Id = 5
            };

            // Act
            var dto = evento.ToDTO();

            // Assert
            dto.Should().NotBeNull();
            dto.Descricao.Should().Be(descricaoLonga);
            dto.Descricao.Length.Should().BeGreaterThan(50);
        }

        [Fact]
        public void EventoMapper_ToDTO_DevePreservarTodosOsCampos()
        {
            // Arrange
            var horarioEspecifico = new DateTime(2025, 6, 15, 9, 0, 0);
            var evento = new Evento("Seminário", "Seminário de Desenvolvimento", horarioEspecifico)
            {
                Id = 10
            };

            // Act
            var dto = evento.ToDTO();

            // Assert
            dto.Id.Should().Be(evento.Id);
            dto.Titulo.Should().Be(evento.Titulo);
            dto.Descricao.Should().Be(evento.Descricao);
            dto.Horario.Should().Be(evento.Horario);
        }

        [Fact]
        public void EventoMapper_ToDTO_DeveConverterEventoComHorarioFuturo()
        {
            // Arrange
            var horarioFuturo = DateTime.Now.AddDays(30);
            var evento = new Evento("Evento Futuro", "Descrição do evento futuro", horarioFuturo)
            {
                Id = 20
            };

            // Act
            var dto = evento.ToDTO();

            // Assert
            dto.Should().NotBeNull();
            dto.Horario.Should().BeAfter(DateTime.Now);
            dto.Horario.Should().Be(horarioFuturo);
        }
    }
}
