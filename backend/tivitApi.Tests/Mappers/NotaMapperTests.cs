using FluentAssertions;
using tivitApi.DTOs;
using tivitApi.Enums;
using tivitApi.Mappers;

namespace tivitApi.Tests.Mappers
{
    public class NotaMapperTests
    {
        [Fact]
        public void ToEntity_DeveConverterDTOParaNota()
        {
            // Arrange
            var dto = new NotaDTORequest
            {
                AlunoId = 1,
                MateriaId = 2,
                Nota1 = 8.0m,
                Nota2 = 7.5m
            };
            var media = 7.75m;
            var status = StatusNota.APROVADO;
            var qntdFaltas = 2;

            // Act
            var entity = dto.ToEntity(media, status, qntdFaltas);

            // Assert
            entity.Should().NotBeNull();
            entity.AlunoId.Should().Be(1);
            entity.MateriaId.Should().Be(2);
            entity.Nota1.Should().Be(8.0m);
            entity.Nota2.Should().Be(7.5m);
            entity.Media.Should().Be(7.75m);
            entity.Status.Should().Be(StatusNota.APROVADO);
            entity.QtdFaltas.Should().Be(2);
        }

        [Fact]
        public void ToEntity_DeveConverterNotaReprovada()
        {
            // Arrange
            var dto = new NotaDTORequest
            {
                AlunoId = 5,
                MateriaId = 10,
                Nota1 = 4.0m,
                Nota2 = 5.0m
            };
            var media = 4.5m;
            var status = StatusNota.REPROVADO;
            var qntdFaltas = 15;

            // Act
            var entity = dto.ToEntity(media, status, qntdFaltas);

            // Assert
            entity.Should().NotBeNull();
            entity.AlunoId.Should().Be(5);
            entity.MateriaId.Should().Be(10);
            entity.Nota1.Should().Be(4.0m);
            entity.Nota2.Should().Be(5.0m);
            entity.Media.Should().Be(4.5m);
            entity.Status.Should().Be(StatusNota.REPROVADO);
            entity.QtdFaltas.Should().Be(15);
        }

        [Fact]
        public void ToEntity_DeveConverterNotaReprovadaComMuitasFaltas()
        {
            // Arrange
            var dto = new NotaDTORequest
            {
                AlunoId = 3,
                MateriaId = 7,
                Nota1 = 5.5m,
                Nota2 = 6.0m
            };
            var media = 5.75m;
            var status = StatusNota.REPROVADO;
            var qntdFaltas = 8;

            // Act
            var entity = dto.ToEntity(media, status, qntdFaltas);

            // Assert
            entity.Should().NotBeNull();
            entity.Status.Should().Be(StatusNota.REPROVADO);
            entity.Media.Should().Be(5.75m);
            entity.QtdFaltas.Should().Be(8);
        }

        [Theory]
        [InlineData(10.0, 10.0, 10.0, StatusNota.APROVADO, 0)]
        [InlineData(7.0, 8.0, 7.5, StatusNota.APROVADO, 3)]
        [InlineData(3.0, 4.0, 3.5, StatusNota.REPROVADO, 20)]
        [InlineData(6.0, 5.0, 5.5, StatusNota.REPROVADO, 10)]
        public void ToEntity_DeveConverterDiferentesNotas(decimal nota1, decimal nota2, decimal media, StatusNota status, int faltas)
        {
            // Arrange
            var dto = new NotaDTORequest
            {
                AlunoId = 1,
                MateriaId = 1,
                Nota1 = nota1,
                Nota2 = nota2
            };

            // Act
            var entity = dto.ToEntity(media, status, faltas);

            // Assert
            entity.Should().NotBeNull();
            entity.Nota1.Should().Be(nota1);
            entity.Nota2.Should().Be(nota2);
            entity.Media.Should().Be(media);
            entity.Status.Should().Be(status);
            entity.QtdFaltas.Should().Be(faltas);
        }

        [Fact]
        public void ToEntity_DeveManterAlunoIdEMateriaId()
        {
            // Arrange
            var dto = new NotaDTORequest
            {
                AlunoId = 999,
                MateriaId = 888,
                Nota1 = 7.0m,
                Nota2 = 8.0m
            };

            // Act
            var entity = dto.ToEntity(7.5m, StatusNota.APROVADO, 0);

            // Assert
            entity.AlunoId.Should().Be(999);
            entity.MateriaId.Should().Be(888);
        }

        [Fact]
        public void ToEntity_DeveAceitarNotasZero()
        {
            // Arrange
            var dto = new NotaDTORequest
            {
                AlunoId = 1,
                MateriaId = 1,
                Nota1 = 0m,
                Nota2 = 0m
            };

            // Act
            var entity = dto.ToEntity(0m, StatusNota.REPROVADO, 25);

            // Assert
            entity.Nota1.Should().Be(0m);
            entity.Nota2.Should().Be(0m);
            entity.Media.Should().Be(0m);
        }

        [Fact]
        public void ToEntity_DeveAceitarNotasMaximas()
        {
            // Arrange
            var dto = new NotaDTORequest
            {
                AlunoId = 1,
                MateriaId = 1,
                Nota1 = 10.0m,
                Nota2 = 10.0m
            };

            // Act
            var entity = dto.ToEntity(10.0m, StatusNota.APROVADO, 0);

            // Assert
            entity.Nota1.Should().Be(10.0m);
            entity.Nota2.Should().Be(10.0m);
            entity.Media.Should().Be(10.0m);
        }

        [Theory]
        [InlineData(StatusNota.APROVADO)]
        [InlineData(StatusNota.REPROVADO)]
        public void ToEntity_DeveAceitarTodosStatusPossiveis(StatusNota status)
        {
            // Arrange
            var dto = new NotaDTORequest
            {
                AlunoId = 1,
                MateriaId = 1,
                Nota1 = 7.0m,
                Nota2 = 7.0m
            };

            // Act
            var entity = dto.ToEntity(7.0m, status, 5);

            // Assert
            entity.Status.Should().Be(status);
        }
    }
}
