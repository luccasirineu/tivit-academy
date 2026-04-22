using FluentAssertions;
using tivitApi.Exceptions;

namespace tivitApi.Tests.Exceptions
{
    public class ExceptionTests
    {
        [Fact]
        public void NotFoundException_DeveSerCriadaComMensagemCorreta()
        {
            // Arrange & Act
            var exception = new NotFoundException("Aluno", 123);

            // Assert
            exception.Message.Should().Contain("Aluno");
            exception.Message.Should().Contain("123");
        }

        [Fact]
        public void NotFoundException_DeveSerCriadaComMensagemPersonalizada()
        {
            // Arrange & Act
            var exception = new NotFoundException("Aluno", "CPF: 12345678900");

            // Assert
            exception.Message.Should().Contain("Aluno");
            exception.Message.Should().Contain("CPF: 12345678900");
        }

        [Fact]
        public void ValidationException_DeveSerCriadaComMensagem()
        {
            // Arrange & Act
            var exception = new ValidationException("Campo obrigatório não preenchido");

            // Assert
            exception.Message.Should().Be("Campo obrigatório não preenchido");
        }

        [Fact]
        public void BusinessException_DeveSerCriadaComMensagem()
        {
            // Arrange & Act
            var exception = new BusinessException("Operação não permitida");

            // Assert
            exception.Message.Should().Be("Operação não permitida");
        }

        [Fact]
        public void CredenciaisInvalidasException_DeveSerCriadaComMensagem()
        {
            // Arrange & Act
            var exception = new CredenciaisInvalidasException("Usuário ou senha inválidos");

            // Assert
            exception.Message.Should().Be("Usuário ou senha inválidos");
        }

        [Fact]
        public void RequisicaoInvalidaException_DeveSerCriadaComMensagem()
        {
            // Arrange & Act
            var exception = new RequisicaoInvalidaException("Requisição inválida");

            // Assert
            exception.Message.Should().Be("Requisição inválida");
        }
    }
}
