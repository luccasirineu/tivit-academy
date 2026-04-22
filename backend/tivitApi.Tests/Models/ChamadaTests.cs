using FluentAssertions;
using tivitApi.Models;

namespace tivitApi.Tests.Models
{
    public class ChamadaTests
    {
        [Fact]
        public void Chamada_DeveCriarInstanciaComConstrutorCompleto()
        {
            // Arrange
            var horario = DateTime.UtcNow;

            // Act
            var chamada = new Chamada(1, 2, false, horario, 3);

            // Assert
            chamada.MatriculaId.Should().Be(1);
            chamada.MateriaId.Should().Be(2);
            chamada.Faltou.Should().BeFalse();
            chamada.HorarioDaAula.Should().Be(horario);
            chamada.TurmaId.Should().Be(3);
        }

        [Fact]
        public void Chamada_DeveRegistrarPresenca()
        {
            // Arrange
            var horario = DateTime.UtcNow;

            // Act
            var chamada = new Chamada(1, 2, false, horario, 3);

            // Assert
            chamada.Faltou.Should().BeFalse();
        }

        [Fact]
        public void Chamada_DeveRegistrarFalta()
        {
            // Arrange
            var horario = DateTime.UtcNow;

            // Act
            var chamada = new Chamada(1, 2, true, horario, 3);

            // Assert
            chamada.Faltou.Should().BeTrue();
        }

        [Fact]
        public void Chamada_DeveArmazenarHorarioDaAula()
        {
            // Arrange
            var horario = new DateTime(2024, 3, 15, 14, 30, 0);

            // Act
            var chamada = new Chamada(1, 2, false, horario, 3);

            // Assert
            chamada.HorarioDaAula.Should().Be(horario);
        }

        [Fact]
        public void Chamada_DeveManterRelacionamentos()
        {
            // Arrange
            var turma = new Turma("Turma A", 1, "ATIVO");
            var matricula = new Matricula("Aluno", "aluno@test.com", "12345678900", 1);
            var materia = new Materia("Matemática", "Descrição", 1);
            var horario = DateTime.UtcNow;

            // Act
            var chamada = new Chamada(matricula.Id, materia.Id, false, horario, turma.Id)
            {
                Turma = turma,
                Matricula = matricula,
                Materia = materia
            };

            // Assert
            chamada.Turma.Should().Be(turma);
            chamada.Matricula.Should().Be(matricula);
            chamada.Materia.Should().Be(materia);
        }
    }
}
