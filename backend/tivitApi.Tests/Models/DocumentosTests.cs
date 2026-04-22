using FluentAssertions;
using tivitApi.Models;

namespace tivitApi.Tests.Models
{
    public class DocumentosTests
    {
        [Fact]
        public void Documentos_DeveCriarInstanciaComConstrutorCompleto()
        {
            // Arrange
            var historico = new byte[] { 1, 2, 3, 4, 5 };
            var cpf = new byte[] { 6, 7, 8, 9, 10 };
            var horaEnvio = DateTime.UtcNow;

            // Act
            var documentos = new Documentos(1, historico, cpf, horaEnvio);

            // Assert
            documentos.MatriculaId.Should().Be(1);
            documentos.DocumentoHistorico.Should().BeEquivalentTo(historico);
            documentos.DocumentoCpf.Should().BeEquivalentTo(cpf);
            documentos.HoraEnvio.Should().Be(horaEnvio);
        }

        [Fact]
        public void Documentos_DeveArmazenarHistoricoCorretamente()
        {
            // Arrange
            var historico = new byte[] { 10, 20, 30 };
            var cpf = new byte[] { 40, 50, 60 };
            var horaEnvio = DateTime.UtcNow;

            // Act
            var documentos = new Documentos(1, historico, cpf, horaEnvio);

            // Assert
            documentos.DocumentoHistorico.Should().HaveCount(3);
            documentos.DocumentoHistorico[0].Should().Be(10);
        }

        [Fact]
        public void Documentos_DeveArmazenarCpfCorretamente()
        {
            // Arrange
            var historico = new byte[] { 1, 2, 3 };
            var cpf = new byte[] { 100, 101, 102 };
            var horaEnvio = DateTime.UtcNow;

            // Act
            var documentos = new Documentos(1, historico, cpf, horaEnvio);

            // Assert
            documentos.DocumentoCpf.Should().HaveCount(3);
            documentos.DocumentoCpf[0].Should().Be(100);
        }

        [Fact]
        public void Documentos_DeveRegistrarHoraDeEnvio()
        {
            // Arrange
            var historico = new byte[] { 1, 2, 3 };
            var cpf = new byte[] { 4, 5, 6 };
            var horaEnvio = new DateTime(2024, 3, 15, 14, 30, 0);

            // Act
            var documentos = new Documentos(1, historico, cpf, horaEnvio);

            // Assert
            documentos.HoraEnvio.Should().Be(horaEnvio);
        }

        [Fact]
        public void Documentos_DeveManterRelacionamentoComMatricula()
        {
            // Arrange
            var matricula = new Matricula("João", "joao@test.com", "12345678900", 1);
            var historico = new byte[] { 1, 2, 3 };
            var cpf = new byte[] { 4, 5, 6 };
            var horaEnvio = DateTime.UtcNow;

            // Act
            var documentos = new Documentos(matricula.Id, historico, cpf, horaEnvio)
            {
                Matricula = matricula
            };

            // Assert
            documentos.Matricula.Should().NotBeNull();
            documentos.Matricula.Should().Be(matricula);
            documentos.MatriculaId.Should().Be(matricula.Id);
        }
    }
}
