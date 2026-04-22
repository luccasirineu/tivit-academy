using FluentAssertions;
using tivitApi.Enums;
using tivitApi.Models;

namespace tivitApi.Tests.Models
{
    public class NotaTests
    {
        [Fact]
        public void Nota_DeveCriarInstanciaComConstrutorCompleto()
        {
            // Arrange & Act
            var nota = new Nota(1, 2, 8.0m, 7.5m, 7.75m, 2, StatusNota.APROVADO);

            // Assert
            nota.AlunoId.Should().Be(1);
            nota.MateriaId.Should().Be(2);
            nota.Nota1.Should().Be(8.0m);
            nota.Nota2.Should().Be(7.5m);
            nota.Media.Should().Be(7.75m);
            nota.QtdFaltas.Should().Be(2);
            nota.Status.Should().Be(StatusNota.APROVADO);
        }

        [Fact]
        public void Nota_DeveCalcularMediaCorretamente()
        {
            // Arrange
            decimal nota1 = 8.0m;
            decimal nota2 = 6.0m;
            decimal mediaEsperada = 7.0m;

            // Act
            var nota = new Nota(1, 1, nota1, nota2, mediaEsperada, 0, StatusNota.APROVADO);

            // Assert
            nota.Media.Should().Be(mediaEsperada);
        }

        [Theory]
        [InlineData(StatusNota.APROVADO)]
        [InlineData(StatusNota.REPROVADO)]
        public void Nota_DeveAceitarTodosOsStatusPossiveis(StatusNota status)
        {
            // Arrange & Act
            var nota = new Nota(1, 1, 7.0m, 7.0m, 7.0m, 0, status);

            // Assert
            nota.Status.Should().Be(status);
        }

        [Fact]
        public void Nota_DeveRegistrarFaltasCorretamente()
        {
            // Arrange & Act
            var nota = new Nota(1, 1, 8.0m, 8.0m, 8.0m, 15, StatusNota.REPROVADO);

            // Assert
            nota.QtdFaltas.Should().Be(15);
        }

        [Fact]
        public void Nota_DeveAceitarNotasDecimais()
        {
            // Arrange & Act
            var nota = new Nota(1, 1, 7.5m, 8.75m, 8.125m, 0, StatusNota.APROVADO);

            // Assert
            nota.Nota1.Should().Be(7.5m);
            nota.Nota2.Should().Be(8.75m);
            nota.Media.Should().Be(8.125m);
        }

        [Fact]
        public void Nota_DeveManterRelacionamentos()
        {
            // Arrange
            var aluno = new Aluno("João", "joao@test.com", "12345678900", "senha", 1);
            var materia = new Materia("Matemática", "Descrição", 1);

            // Act
            var nota = new Nota(aluno.Id, materia.Id, 8.0m, 7.0m, 7.5m, 2, StatusNota.APROVADO)
            {
                Aluno = aluno,
                Materia = materia
            };

            // Assert
            nota.Aluno.Should().Be(aluno);
            nota.Materia.Should().Be(materia);
        }

        [Fact]
        public void Nota_DeveAceitarNotaZero()
        {
            // Arrange & Act
            var nota = new Nota(1, 1, 0m, 0m, 0m, 25, StatusNota.REPROVADO);

            // Assert
            nota.Nota1.Should().Be(0m);
            nota.Nota2.Should().Be(0m);
            nota.Media.Should().Be(0m);
        }

        [Fact]
        public void Nota_DeveAceitarNotaDez()
        {
            // Arrange & Act
            var nota = new Nota(1, 1, 10.0m, 10.0m, 10.0m, 0, StatusNota.APROVADO);

            // Assert
            nota.Nota1.Should().Be(10.0m);
            nota.Nota2.Should().Be(10.0m);
            nota.Media.Should().Be(10.0m);
        }
    }
}
