using FluentAssertions;
using tivitApi.Models;

namespace tivitApi.Tests.Models
{
    public class ConteudoTests
    {
        [Fact]
        public void Conteudo_DeveCriarInstanciaComPropriedadesCorretas()
        {
            // Arrange
            var dataPublicacao = DateTime.UtcNow;

            // Act
            var conteudo = new Conteudo
            {
                Id = 1,
                Titulo = "Aula 1 - Introdução",
                Tipo = "PDF",
                CaminhoOuUrl = "/uploads/aula1.pdf",
                DataPublicacao = dataPublicacao,
                MateriaId = 1,
                ProfessorId = 2,
                TurmaId = 3
            };

            // Assert
            conteudo.Id.Should().Be(1);
            conteudo.Titulo.Should().Be("Aula 1 - Introdução");
            conteudo.Tipo.Should().Be("PDF");
            conteudo.CaminhoOuUrl.Should().Be("/uploads/aula1.pdf");
            conteudo.DataPublicacao.Should().Be(dataPublicacao);
            conteudo.MateriaId.Should().Be(1);
            conteudo.ProfessorId.Should().Be(2);
            conteudo.TurmaId.Should().Be(3);
        }

        [Theory]
        [InlineData("PDF")]
        [InlineData("LINK")]
        [InlineData("VIDEO")]
        public void Conteudo_DeveAceitarDiferentesTipos(string tipo)
        {
            // Arrange & Act
            var conteudo = new Conteudo
            {
                Titulo = "Conteúdo Teste",
                Tipo = tipo,
                CaminhoOuUrl = "/path",
                DataPublicacao = DateTime.UtcNow,
                MateriaId = 1,
                ProfessorId = 1,
                TurmaId = 1
            };

            // Assert
            conteudo.Tipo.Should().Be(tipo);
        }

        [Fact]
        public void Conteudo_DeveArmazenarUrlCorretamente()
        {
            // Arrange
            var url = "https://example.com/video";

            // Act
            var conteudo = new Conteudo
            {
                Titulo = "Vídeo Aula",
                Tipo = "LINK",
                CaminhoOuUrl = url,
                DataPublicacao = DateTime.UtcNow,
                MateriaId = 1,
                ProfessorId = 1,
                TurmaId = 1
            };

            // Assert
            conteudo.CaminhoOuUrl.Should().Be(url);
        }

        [Fact]
        public void Conteudo_DeveManterRelacionamentos()
        {
            // Arrange
            var materia = new Materia("Matemática", "Descrição", 1);
            var professor = new Professor("Prof. João", "prof@test.com", "RM123", "12345678900", tivitApi.Enums.StatusUsuario.ATIVO);

            // Act
            var conteudo = new Conteudo
            {
                Titulo = "Aula 1",
                Tipo = "PDF",
                CaminhoOuUrl = "/path",
                DataPublicacao = DateTime.UtcNow,
                MateriaId = materia.Id,
                ProfessorId = professor.Id,
                TurmaId = 1,
                Materia = materia,
                Professor = professor
            };

            // Assert
            conteudo.Materia.Should().Be(materia);
            conteudo.Professor.Should().Be(professor);
        }
    }
}
