using FluentAssertions;
using tivitApi.DTOs;
using tivitApi.Mappers;

namespace tivitApi.Tests.Mappers
{
    public class MateriaMapperTests
    {
        [Fact]
        public void MateriaMapper_ToEntity_DeveConverterCorretamente()
        {
            // Arrange
            var dto = new MateriaDTO
            {
                Nome = "Matemática Avançada",
                Descricao = "Curso de cálculo diferencial e integral",
                CursoId = 5
            };

            // Act
            var entity = dto.ToEntity();

            // Assert
            entity.Should().NotBeNull();
            entity.Nome.Should().Be("Matemática Avançada");
            entity.Descricao.Should().Be("Curso de cálculo diferencial e integral");
            entity.CursoId.Should().Be(5);
        }

        [Fact]
        public void MateriaMapper_ToEntity_DeveConverterComDescricaoCurta()
        {
            // Arrange
            var dto = new MateriaDTO
            {
                Nome = "Física",
                Descricao = "Física básica",
                CursoId = 10
            };

            // Act
            var entity = dto.ToEntity();

            // Assert
            entity.Should().NotBeNull();
            entity.Nome.Should().Be("Física");
            entity.Descricao.Should().Be("Física básica");
            entity.CursoId.Should().Be(10);
        }

        [Fact]
        public void MateriaMapper_ToEntity_DeveConverterComDescricaoLonga()
        {
            // Arrange
            var descricaoLonga = "Esta é uma descrição muito detalhada da matéria que inclui " +
                                "informações sobre o conteúdo programático, objetivos de aprendizagem, " +
                                "metodologia de ensino e critérios de avaliação.";
            var dto = new MateriaDTO
            {
                Nome = "Programação Orientada a Objetos",
                Descricao = descricaoLonga,
                CursoId = 3
            };

            // Act
            var entity = dto.ToEntity();

            // Assert
            entity.Should().NotBeNull();
            entity.Descricao.Should().Be(descricaoLonga);
            entity.Descricao.Length.Should().BeGreaterThan(100);
        }

        [Fact]
        public void MateriaMapper_ToEntity_DevePreservarTodosOsCampos()
        {
            // Arrange
            var dto = new MateriaDTO
            {
                Nome = "Banco de Dados",
                Descricao = "Modelagem e implementação de bancos de dados relacionais",
                CursoId = 7
            };

            // Act
            var entity = dto.ToEntity();

            // Assert
            entity.Nome.Should().Be(dto.Nome);
            entity.Descricao.Should().Be(dto.Descricao);
            entity.CursoId.Should().Be(dto.CursoId);
        }

        [Fact]
        public void MateriaMapper_ToEntity_DeveConverterComNomeComCaracteresEspeciais()
        {
            // Arrange
            var dto = new MateriaDTO
            {
                Nome = "Lógica & Algoritmos",
                Descricao = "Introdução à lógica de programação",
                CursoId = 1
            };

            // Act
            var entity = dto.ToEntity();

            // Assert
            entity.Should().NotBeNull();
            entity.Nome.Should().Contain("&");
            entity.Nome.Should().Be("Lógica & Algoritmos");
        }
    }
}
