using FluentAssertions;
using tivitApi.Enums;
using tivitApi.Models;

namespace tivitApi.Tests.Models
{
    public class MateriaTests
    {
        [Fact]
        public void Materia_DeveCriarInstanciaComConstrutorCompleto()
        {
            // Arrange & Act
            var materia = new Materia("Matemática", "Cálculo Diferencial", 1);

            // Assert
            materia.Nome.Should().Be("Matemática");
            materia.Descricao.Should().Be("Cálculo Diferencial");
            materia.CursoId.Should().Be(1);
        }

        [Fact]
        public void Materia_DeveCriarInstanciaComConstrutorVazio()
        {
            // Arrange & Act
            var materia = new Materia();

            // Assert
            materia.Should().NotBeNull();
        }

        [Fact]
        public void Materia_DeveManterRelacionamentoComCurso()
        {
            // Arrange
            var curso = new Curso("Engenharia", "Curso de Engenharia", 1, StatusCurso.ATIVO);

            // Act
            var materia = new Materia("Física", "Física Aplicada", curso.Id)
            {
                curso = curso
            };

            // Assert
            materia.curso.Should().NotBeNull();
            materia.curso.Should().Be(curso);
            materia.CursoId.Should().Be(curso.Id);
        }

        [Fact]
        public void Materia_DevePermitirColecaoDeConteudos()
        {
            // Arrange
            var materia = new Materia("Química", "Química Orgânica", 1);
            var conteudos = new List<Conteudo>
            {
                new Conteudo { Titulo = "Aula 1", Tipo = "PDF", CaminhoOuUrl = "/path1", DataPublicacao = DateTime.UtcNow, MateriaId = materia.Id, ProfessorId = 1, TurmaId = 1 },
                new Conteudo { Titulo = "Aula 2", Tipo = "PDF", CaminhoOuUrl = "/path2", DataPublicacao = DateTime.UtcNow, MateriaId = materia.Id, ProfessorId = 1, TurmaId = 1 }
            };

            // Act
            materia.Conteudos = conteudos;

            // Assert
            materia.Conteudos.Should().HaveCount(2);
            materia.Conteudos.Should().Contain(c => c.Titulo == "Aula 1");
        }

        [Fact]
        public void Materia_DevePermitirAlteracaoDePropriedades()
        {
            // Arrange
            var materia = new Materia("Nome Original", "Descrição Original", 1);

            // Act
            materia.Nome = "Novo Nome";
            materia.Descricao = "Nova Descrição";

            // Assert
            materia.Nome.Should().Be("Novo Nome");
            materia.Descricao.Should().Be("Nova Descrição");
        }
    }
}
