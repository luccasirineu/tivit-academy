using FluentAssertions;
using tivitApi.DTOs;
using tivitApi.Mappers;
using tivitApi.Models;

namespace tivitApi.Tests.Mappers
{
    public class TurmaMapperTests
    {
        [Fact]
        public void ToDTO_DeveConverterTurmaParaDTO()
        {
            // Arrange
            var turma = new Turma("Turma A", 2, "ATIVO")
            {
                Id = 1
            };

            // Act
            var dto = turma.ToDTO();

            // Assert
            dto.Should().NotBeNull();
            dto.Id.Should().Be(1);
            dto.Nome.Should().Be("Turma A");
            dto.CursoId.Should().Be(2);
            dto.Status.Should().Be("ATIVO");
        }

        [Fact]
        public void ToDTO_DeveConverterTurmaInativaParaDTO()
        {
            // Arrange
            var turma = new Turma("Turma B", 5, "INATIVO")
            {
                Id = 10
            };

            // Act
            var dto = turma.ToDTO();

            // Assert
            dto.Should().NotBeNull();
            dto.Id.Should().Be(10);
            dto.Nome.Should().Be("Turma B");
            dto.CursoId.Should().Be(5);
            dto.Status.Should().Be("INATIVO");
        }

        [Fact]
        public void ToEntity_DeveConverterDTOParaTurma()
        {
            // Arrange
            var dto = new TurmaDTORequest
            {
                Nome = "Turma C",
                CursoId = 3,
                Status = "ATIVO"
            };

            // Act
            var entity = dto.ToEntity();

            // Assert
            entity.Should().NotBeNull();
            entity.Nome.Should().Be("Turma C");
            entity.CursoId.Should().Be(3);
            entity.Status.Should().Be("ATIVO");
        }

        [Fact]
        public void ToEntity_DeveConverterDTOComStatusInativo()
        {
            // Arrange
            var dto = new TurmaDTORequest
            {
                Nome = "Turma D",
                CursoId = 7,
                Status = "INATIVO"
            };

            // Act
            var entity = dto.ToEntity();

            // Assert
            entity.Should().NotBeNull();
            entity.Nome.Should().Be("Turma D");
            entity.CursoId.Should().Be(7);
            entity.Status.Should().Be("INATIVO");
        }

        [Theory]
        [InlineData("Turma 1", 1, "ATIVO")]
        [InlineData("Turma 2", 2, "INATIVO")]
        [InlineData("Turma Especial", 99, "ATIVO")]
        public void ToDTO_DeveConverterDiferentesTurmas(string nome, int cursoId, string status)
        {
            // Arrange
            var turma = new Turma(nome, cursoId, status)
            {
                Id = 100
            };

            // Act
            var dto = turma.ToDTO();

            // Assert
            dto.Should().NotBeNull();
            dto.Nome.Should().Be(nome);
            dto.CursoId.Should().Be(cursoId);
            dto.Status.Should().Be(status);
        }

        [Theory]
        [InlineData("Turma X", 5, "ATIVO")]
        [InlineData("Turma Y", 10, "INATIVO")]
        public void ToEntity_DeveConverterDiferentesDTOs(string nome, int cursoId, string status)
        {
            // Arrange
            var dto = new TurmaDTORequest
            {
                Nome = nome,
                CursoId = cursoId,
                Status = status
            };

            // Act
            var entity = dto.ToEntity();

            // Assert
            entity.Should().NotBeNull();
            entity.Nome.Should().Be(nome);
            entity.CursoId.Should().Be(cursoId);
            entity.Status.Should().Be(status);
        }

        [Fact]
        public void ToDTO_DeveManterIdOriginal()
        {
            // Arrange
            var turma = new Turma("Turma Test", 1, "ATIVO")
            {
                Id = 999
            };

            // Act
            var dto = turma.ToDTO();

            // Assert
            dto.Id.Should().Be(999);
        }
    }
}
