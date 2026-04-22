using FluentAssertions;
using tivitApi.DTOs;
using tivitApi.Mappers;

namespace tivitApi.Tests.Mappers
{
    public class ChamadaMapperTests
    {
        [Fact]
        public void ChamadaMapper_ToEntity_DeveConverterCorretamente()
        {
            // Arrange
            var dto = new ChamadaDTO
            {
                MatriculaId = 1,
                MateriaId = 2,
                Faltou = true,
                TurmaId = 3
            };

            // Act
            var entity = dto.ToEntity();

            // Assert
            entity.Should().NotBeNull();
            entity.MatriculaId.Should().Be(1);
            entity.MateriaId.Should().Be(2);
            entity.Faltou.Should().BeTrue();
            entity.TurmaId.Should().Be(3);
            entity.HorarioDaAula.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void ChamadaMapper_ToEntity_DeveCriarComFaltouFalse()
        {
            // Arrange
            var dto = new ChamadaDTO
            {
                MatriculaId = 10,
                MateriaId = 20,
                Faltou = false,
                TurmaId = 30
            };

            // Act
            var entity = dto.ToEntity();

            // Assert
            entity.Should().NotBeNull();
            entity.Faltou.Should().BeFalse();
            entity.MatriculaId.Should().Be(10);
            entity.MateriaId.Should().Be(20);
            entity.TurmaId.Should().Be(30);
        }

        [Fact]
        public void ChamadaMapper_ToEntity_DeveDefinirHorarioUtc()
        {
            // Arrange
            var dto = new ChamadaDTO
            {
                MatriculaId = 1,
                MateriaId = 2,
                Faltou = true,
                TurmaId = 3
            };
            var antes = DateTime.UtcNow;

            // Act
            var entity = dto.ToEntity();
            var depois = DateTime.UtcNow;

            // Assert
            entity.HorarioDaAula.Should().BeOnOrAfter(antes);
            entity.HorarioDaAula.Should().BeOnOrBefore(depois);
        }
    }
}
