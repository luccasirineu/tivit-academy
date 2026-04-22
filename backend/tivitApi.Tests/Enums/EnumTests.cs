using FluentAssertions;
using tivitApi.Enums;

namespace tivitApi.Tests.Enums
{
    public class EnumTests
    {
        [Fact]
        public void StatusCurso_DeveTerValoresCorretos()
        {
            // Assert
            Enum.GetNames(typeof(StatusCurso)).Should().Contain(new[] { "ATIVO", "DESATIVADO" });
            Enum.GetNames(typeof(StatusCurso)).Should().HaveCount(2);
        }

        [Fact]
        public void StatusMatricula_DeveTerValoresCorretos()
        {
            // Assert
            var valores = Enum.GetNames(typeof(StatusMatricula));
            valores.Should().Contain("AGUARDANDO_PAGAMENTO");
            valores.Should().Contain("AGUARDANDO_DOCUMENTOS");
            valores.Should().Contain("AGUARDANDO_APROVACAO");
            valores.Should().Contain("APROVADO");
            valores.Should().Contain("RECUSADO");
            valores.Should().HaveCount(5);
        }

        [Fact]
        public void StatusNota_DeveTerValoresCorretos()
        {
            // Assert
            Enum.GetNames(typeof(StatusNota)).Should().Contain(new[] { "APROVADO", "REPROVADO" });
            Enum.GetNames(typeof(StatusNota)).Should().HaveCount(2);
        }

        [Fact]
        public void StatusUsuario_DeveTerValoresCorretos()
        {
            // Assert
            var valores = Enum.GetNames(typeof(StatusUsuario));
            valores.Should().Contain("ATIVO");
            valores.Should().Contain("DESATIVADO");
            valores.Should().Contain("BLOQUEADO");
            valores.Should().HaveCount(3);
        }

        [Theory]
        [InlineData(StatusCurso.ATIVO, "ATIVO")]
        [InlineData(StatusCurso.DESATIVADO, "DESATIVADO")]
        public void StatusCurso_DeveConverterParaStringCorretamente(StatusCurso status, string esperado)
        {
            // Act
            var resultado = status.ToString();

            // Assert
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData(StatusMatricula.AGUARDANDO_PAGAMENTO, "AGUARDANDO_PAGAMENTO")]
        [InlineData(StatusMatricula.APROVADO, "APROVADO")]
        [InlineData(StatusMatricula.RECUSADO, "RECUSADO")]
        public void StatusMatricula_DeveConverterParaStringCorretamente(StatusMatricula status, string esperado)
        {
            // Act
            var resultado = status.ToString();

            // Assert
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData(StatusNota.APROVADO, "APROVADO")]
        [InlineData(StatusNota.REPROVADO, "REPROVADO")]
        public void StatusNota_DeveConverterParaStringCorretamente(StatusNota status, string esperado)
        {
            // Act
            var resultado = status.ToString();

            // Assert
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData(StatusUsuario.ATIVO, "ATIVO")]
        [InlineData(StatusUsuario.DESATIVADO, "DESATIVADO")]
        [InlineData(StatusUsuario.BLOQUEADO, "BLOQUEADO")]
        public void StatusUsuario_DeveConverterParaStringCorretamente(StatusUsuario status, string esperado)
        {
            // Act
            var resultado = status.ToString();

            // Assert
            resultado.Should().Be(esperado);
        }
    }
}
