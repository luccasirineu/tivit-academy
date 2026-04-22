using FluentAssertions;
using tivitApi.DTOs;
using tivitApi.Enums;
using tivitApi.Mappers;
using tivitApi.Models;

namespace tivitApi.Tests.Mappers
{
    public class CursoMapperTests
    {
        [Fact]
        public void ToDTO_DeveConverterCursoParaDTO()
        {
            // Arrange
            var curso = new Curso("Engenharia", "Curso de Engenharia", 5, StatusCurso.ATIVO)
            {
                Id = 1
            };

            // Act
            var dto = curso.ToDTO();

            // Assert
            dto.Should().NotBeNull();
            dto.Id.Should().Be(1);
            dto.Nome.Should().Be("Engenharia");
            dto.Descricao.Should().Be("Curso de Engenharia");
            dto.ProfResponsavel.Should().Be(5);
            dto.Status.Should().Be("ATIVO");
        }

        [Fact]
        public void ToDTO_DeveConverterCursoDesativadoParaDTO()
        {
            // Arrange
            var curso = new Curso("Medicina", "Curso de Medicina", 3, StatusCurso.DESATIVADO)
            {
                Id = 2
            };

            // Act
            var dto = curso.ToDTO();

            // Assert
            dto.Should().NotBeNull();
            dto.Id.Should().Be(2);
            dto.Nome.Should().Be("Medicina");
            dto.Descricao.Should().Be("Curso de Medicina");
            dto.ProfResponsavel.Should().Be(3);
            dto.Status.Should().Be("DESATIVADO");
        }

        [Fact]
        public void ToEntity_DeveConverterDTOParaCurso()
        {
            // Arrange
            var dto = new CursoDTORequest
            {
                Nome = "Direito",
                Descricao = "Curso de Direito",
                ProfResponsavel = 7
            };

            // Act
            var entity = dto.ToEntity();

            // Assert
            entity.Should().NotBeNull();
            entity.Nome.Should().Be("Direito");
            entity.Descricao.Should().Be("Curso de Direito");
            entity.ProfResponsavel.Should().Be(7);
            entity.Status.Should().Be(StatusCurso.ATIVO);
        }

        [Fact]
        public void ToEntity_DeveCriarCursoComStatusAtivoPorPadrao()
        {
            // Arrange
            var dto = new CursoDTORequest
            {
                Nome = "Arquitetura",
                Descricao = "Curso de Arquitetura",
                ProfResponsavel = 10
            };

            // Act
            var entity = dto.ToEntity();

            // Assert
            entity.Status.Should().Be(StatusCurso.ATIVO);
        }

        [Theory]
        [InlineData("Engenharia Civil", "Curso completo de Engenharia Civil", 1, StatusCurso.ATIVO)]
        [InlineData("Administração", "Curso de Administração de Empresas", 2, StatusCurso.DESATIVADO)]
        [InlineData("Psicologia", "Curso de Psicologia Clínica", 3, StatusCurso.ATIVO)]
        public void ToDTO_DeveConverterDiferentesCursos(string nome, string descricao, int profId, StatusCurso status)
        {
            // Arrange
            var curso = new Curso(nome, descricao, profId, status)
            {
                Id = 100
            };

            // Act
            var dto = curso.ToDTO();

            // Assert
            dto.Should().NotBeNull();
            dto.Nome.Should().Be(nome);
            dto.Descricao.Should().Be(descricao);
            dto.ProfResponsavel.Should().Be(profId);
            dto.Status.Should().Be(status.ToString());
        }

        [Theory]
        [InlineData("Matemática", "Curso de Matemática Aplicada", 5)]
        [InlineData("Física", "Curso de Física Quântica", 8)]
        public void ToEntity_DeveConverterDiferentesDTOs(string nome, string descricao, int profId)
        {
            // Arrange
            var dto = new CursoDTORequest
            {
                Nome = nome,
                Descricao = descricao,
                ProfResponsavel = profId
            };

            // Act
            var entity = dto.ToEntity();

            // Assert
            entity.Should().NotBeNull();
            entity.Nome.Should().Be(nome);
            entity.Descricao.Should().Be(descricao);
            entity.ProfResponsavel.Should().Be(profId);
        }

        [Fact]
        public void ToDTO_DeveManterIdOriginal()
        {
            // Arrange
            var curso = new Curso("Test", "Test Description", 1, StatusCurso.ATIVO)
            {
                Id = 999
            };

            // Act
            var dto = curso.ToDTO();

            // Assert
            dto.Id.Should().Be(999);
        }

        [Fact]
        public void ToDTO_DeveConverterStatusParaString()
        {
            // Arrange
            var cursoAtivo = new Curso("Curso 1", "Desc 1", 1, StatusCurso.ATIVO) { Id = 1 };
            var cursoDesativado = new Curso("Curso 2", "Desc 2", 2, StatusCurso.DESATIVADO) { Id = 2 };

            // Act
            var dtoAtivo = cursoAtivo.ToDTO();
            var dtoDesativado = cursoDesativado.ToDTO();

            // Assert
            dtoAtivo.Status.Should().Be("ATIVO");
            dtoDesativado.Status.Should().Be("DESATIVADO");
        }
    }
}
