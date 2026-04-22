using FluentAssertions;
using tivitApi.Models;

namespace tivitApi.Tests.Models
{
    public class ComprovantePagamentoTests
    {
        [Fact]
        public void ComprovantePagamento_DeveCriarInstanciaComConstrutorCompleto()
        {
            // Arrange
            var arquivo = new byte[] { 1, 2, 3, 4, 5 };
            var horaEnvio = DateTime.UtcNow;

            // Act
            var comprovante = new ComprovantePagamento(1, arquivo, horaEnvio);

            // Assert
            comprovante.MatriculaId.Should().Be(1);
            comprovante.Arquivo.Should().BeEquivalentTo(arquivo);
            comprovante.HoraEnvio.Should().Be(horaEnvio);
        }

        [Fact]
        public void ComprovantePagamento_DeveArmazenarArquivoCorretamente()
        {
            // Arrange
            var arquivo = new byte[] { 10, 20, 30, 40, 50 };
            var horaEnvio = DateTime.UtcNow;

            // Act
            var comprovante = new ComprovantePagamento(1, arquivo, horaEnvio);

            // Assert
            comprovante.Arquivo.Should().HaveCount(5);
            comprovante.Arquivo[0].Should().Be(10);
            comprovante.Arquivo[4].Should().Be(50);
        }

        [Fact]
        public void ComprovantePagamento_DeveRegistrarHoraDeEnvio()
        {
            // Arrange
            var arquivo = new byte[] { 1, 2, 3 };
            var horaEnvio = new DateTime(2024, 3, 15, 10, 30, 0);

            // Act
            var comprovante = new ComprovantePagamento(1, arquivo, horaEnvio);

            // Assert
            comprovante.HoraEnvio.Should().Be(horaEnvio);
        }

        [Fact]
        public void ComprovantePagamento_DeveManterRelacionamentoComMatricula()
        {
            // Arrange
            var matricula = new Matricula("João", "joao@test.com", "12345678900", 1);
            var arquivo = new byte[] { 1, 2, 3 };
            var horaEnvio = DateTime.UtcNow;

            // Act
            var comprovante = new ComprovantePagamento(matricula.Id, arquivo, horaEnvio)
            {
                Matricula = matricula
            };

            // Assert
            comprovante.Matricula.Should().NotBeNull();
            comprovante.Matricula.Should().Be(matricula);
            comprovante.MatriculaId.Should().Be(matricula.Id);
        }
    }
}
