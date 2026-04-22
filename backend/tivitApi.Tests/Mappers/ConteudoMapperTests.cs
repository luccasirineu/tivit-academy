using FluentAssertions;
using tivitApi.Mappers;
using tivitApi.Models;

namespace tivitApi.Tests.Mappers
{
    public class ConteudoMapperTests
    {
        [Fact]
        public void ConteudoMapper_ToDTO_DeveConverterCorretamente()
        {
            // Arrange
            var dataPublicacao = DateTime.Now;
            var conteudo = new Conteudo
            {
                Id = 1,
                Titulo = "Introdução ao C#",
                Tipo = "PDF",
                CaminhoOuUrl = "/conteudos/intro-csharp.pdf",
                DataPublicacao = dataPublicacao,
                MateriaId = 5,
                ProfessorId = 10,
                TurmaId = 15
            };

            // Act
            var dto = conteudo.ToDTO();

            // Assert
            dto.Should().NotBeNull();
            dto.Id.Should().Be(1);
            dto.Titulo.Should().Be("Introdução ao C#");
            dto.Tipo.Should().Be("PDF");
            dto.CaminhoOuUrl.Should().Be("/conteudos/intro-csharp.pdf");
            dto.DataPublicacao.Should().Be(dataPublicacao);
            dto.MateriaId.Should().Be(5);
            dto.ProfessorId.Should().Be(10);
            dto.TurmaId.Should().Be(15);
        }

        [Fact]
        public void ConteudoMapper_ToDTO_DeveConverterConteudoTipoLink()
        {
            // Arrange
            var conteudo = new Conteudo
            {
                Id = 2,
                Titulo = "Vídeo Aula",
                Tipo = "LINK",
                CaminhoOuUrl = "https://youtube.com/watch?v=123",
                DataPublicacao = DateTime.Now,
                MateriaId = 3,
                ProfessorId = 7,
                TurmaId = 9
            };

            // Act
            var dto = conteudo.ToDTO();

            // Assert
            dto.Should().NotBeNull();
            dto.Tipo.Should().Be("LINK");
            dto.CaminhoOuUrl.Should().StartWith("https://");
        }

        [Fact]
        public void ConteudoMapper_ToDTO_DevePreservarTodosOsCampos()
        {
            // Arrange
            var dataEspecifica = new DateTime(2024, 1, 15, 10, 30, 0);
            var conteudo = new Conteudo
            {
                Id = 100,
                Titulo = "Material Completo",
                Tipo = "PDF",
                CaminhoOuUrl = "/path/to/file.pdf",
                DataPublicacao = dataEspecifica,
                MateriaId = 50,
                ProfessorId = 25,
                TurmaId = 75
            };

            // Act
            var dto = conteudo.ToDTO();

            // Assert
            dto.Id.Should().Be(conteudo.Id);
            dto.Titulo.Should().Be(conteudo.Titulo);
            dto.Tipo.Should().Be(conteudo.Tipo);
            dto.CaminhoOuUrl.Should().Be(conteudo.CaminhoOuUrl);
            dto.DataPublicacao.Should().Be(conteudo.DataPublicacao);
            dto.MateriaId.Should().Be(conteudo.MateriaId);
            dto.ProfessorId.Should().Be(conteudo.ProfessorId);
            dto.TurmaId.Should().Be(conteudo.TurmaId);
        }
    }
}
