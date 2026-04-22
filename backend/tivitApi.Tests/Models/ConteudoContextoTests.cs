using FluentAssertions;
using tivitApi.Models;

namespace tivitApi.Tests.Models
{
    public class ConteudoContextoTests
    {
        [Fact]
        public void ConteudoContexto_DeveCriarInstanciaComPropriedadesCorretas()
        {
            // Arrange
            var dataArmazenamento = DateTime.UtcNow;

            // Act
            var contexto = new ConteudoContexto
            {
                Id = 1,
                ConteudoId = 10,
                ContextoTexto = "Texto extraído do PDF",
                DataArmazenamento = dataArmazenamento,
                StatusExtracao = "sucesso",
                TurmaId = 5
            };

            // Assert
            contexto.Id.Should().Be(1);
            contexto.ConteudoId.Should().Be(10);
            contexto.ContextoTexto.Should().Be("Texto extraído do PDF");
            contexto.DataArmazenamento.Should().Be(dataArmazenamento);
            contexto.StatusExtracao.Should().Be("sucesso");
            contexto.TurmaId.Should().Be(5);
        }

        [Fact]
        public void ConteudoContexto_DeveInicializarComStatusSucessoPorPadrao()
        {
            // Arrange & Act
            var contexto = new ConteudoContexto
            {
                ConteudoId = 1,
                ContextoTexto = "Texto",
                DataArmazenamento = DateTime.UtcNow,
                TurmaId = 1
            };

            // Assert
            contexto.StatusExtracao.Should().Be("sucesso");
        }

        [Fact]
        public void ConteudoContexto_DevePermitirArmazenarMensagemDeErro()
        {
            // Arrange & Act
            var contexto = new ConteudoContexto
            {
                ConteudoId = 1,
                ContextoTexto = "",
                DataArmazenamento = DateTime.UtcNow,
                StatusExtracao = "erro",
                MensagemErro = "Falha ao extrair texto do PDF",
                TurmaId = 1
            };

            // Assert
            contexto.StatusExtracao.Should().Be("erro");
            contexto.MensagemErro.Should().Be("Falha ao extrair texto do PDF");
        }

        [Fact]
        public void ConteudoContexto_DeveManterRelacionamentoComConteudo()
        {
            // Arrange
            var conteudo = new Conteudo
            {
                Titulo = "Aula 1",
                Tipo = "PDF",
                CaminhoOuUrl = "/path",
                DataPublicacao = DateTime.UtcNow,
                MateriaId = 1,
                ProfessorId = 1,
                TurmaId = 1
            };

            // Act
            var contexto = new ConteudoContexto
            {
                ConteudoId = conteudo.Id,
                Conteudo = conteudo,
                ContextoTexto = "Texto extraído",
                DataArmazenamento = DateTime.UtcNow,
                TurmaId = 1
            };

            // Assert
            contexto.Conteudo.Should().Be(conteudo);
            contexto.ConteudoId.Should().Be(conteudo.Id);
        }

        [Fact]
        public void ConteudoContexto_MensagemErroDeveSerNullQuandoSucesso()
        {
            // Arrange & Act
            var contexto = new ConteudoContexto
            {
                ConteudoId = 1,
                ContextoTexto = "Texto",
                DataArmazenamento = DateTime.UtcNow,
                StatusExtracao = "sucesso",
                TurmaId = 1
            };

            // Assert
            contexto.MensagemErro.Should().BeNull();
        }
    }
}
