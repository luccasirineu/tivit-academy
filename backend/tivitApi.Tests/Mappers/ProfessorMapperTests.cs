using FluentAssertions;
using tivitApi.DTOs;
using tivitApi.Enums;
using tivitApi.Mappers;
using tivitApi.Models;

namespace tivitApi.Tests.Mappers
{
    public class ProfessorMapperTests
    {
        [Fact]
        public void ProfessorMapper_ToDTO_DeveConverterCorretamente()
        {
            // Arrange
            var professor = new Professor
            {
                Id = 1,
                Nome = "Dr. João Silva",
                Email = "joao.silva@universidade.com",
                Rm = "RM123456",
                Cpf = "12345678900",
                Senha = "senhaHash123",
                Status = StatusUsuario.ATIVO
            };

            // Act
            var dto = professor.ToDTO();

            // Assert
            dto.Should().NotBeNull();
            dto.Id.Should().Be(1);
            dto.Nome.Should().Be("Dr. João Silva");
            dto.Email.Should().Be("joao.silva@universidade.com");
            dto.Rm.Should().Be("RM123456");
            dto.Cpf.Should().Be("12345678900");
            dto.Status.Should().Be("ATIVO");
        }

        [Fact]
        public void ProfessorMapper_ToDTO_NaoDeveExporSenha()
        {
            // Arrange
            var professor = new Professor
            {
                Id = 2,
                Nome = "Profa. Maria Santos",
                Email = "maria@universidade.com",
                Rm = "RM654321",
                Cpf = "98765432100",
                Senha = "senhaSecreta",
                Status = StatusUsuario.ATIVO
            };

            // Act
            var dto = professor.ToDTO();

            // Assert
            dto.Should().NotBeNull();
            // O DTO não deve ter propriedade Senha
            dto.GetType().GetProperty("Senha").Should().BeNull();
        }

        [Fact]
        public void ProfessorMapper_ToDTO_DeveConverterStatusParaString()
        {
            // Arrange
            var professor = new Professor
            {
                Id = 3,
                Nome = "Prof. Carlos",
                Email = "carlos@universidade.com",
                Rm = "RM111222",
                Cpf = "11122233344",
                Senha = "hash",
                Status = StatusUsuario.DESATIVADO
            };

            // Act
            var dto = professor.ToDTO();

            // Assert
            dto.Status.Should().Be("DESATIVADO");
            dto.Status.Should().BeOfType<string>();
        }

        [Fact]
        public void ProfessorMapper_ToEntity_DeveConverterCorretamente()
        {
            // Arrange
            var dto = new ProfessorDTORequest
            {
                Nome = "Prof. Ana Costa",
                Email = "ana.costa@universidade.com",
                Cpf = "55566677788",
                Status = "ATIVO"
            };
            var senha = "senhaHasheada123";
            var rm = "RM999888";

            // Act
            var entity = dto.ToEntity(senha, rm);

            // Assert
            entity.Should().NotBeNull();
            entity.Nome.Should().Be("Prof. Ana Costa");
            entity.Email.Should().Be("ana.costa@universidade.com");
            entity.Cpf.Should().Be("55566677788");
            entity.Rm.Should().Be("RM999888");
            entity.Senha.Should().Be("senhaHasheada123");
            entity.Status.Should().Be(StatusUsuario.ATIVO);
        }

        [Fact]
        public void ProfessorMapper_ToEntity_DeveSempreDefinirStatusComoAtivo()
        {
            // Arrange
            var dto = new ProfessorDTORequest
            {
                Nome = "Prof. Pedro",
                Email = "pedro@universidade.com",
                Cpf = "12312312312",
                Status = "QUALQUER_COISA" // Independente do que vier no DTO
            };

            // Act
            var entity = dto.ToEntity("senha123", "RM123");

            // Assert
            entity.Status.Should().Be(StatusUsuario.ATIVO);
        }

        [Fact]
        public void ProfessorMapper_ToEntity_DeveUsarSenhaERmFornecidos()
        {
            // Arrange
            var dto = new ProfessorDTORequest
            {
                Nome = "Prof. Teste",
                Email = "teste@universidade.com",
                Cpf = "99999999999",
                Status = "ATIVO"
            };
            var senhaEsperada = "hashComplexo$2024";
            var rmEsperado = "RM777666";

            // Act
            var entity = dto.ToEntity(senhaEsperada, rmEsperado);

            // Assert
            entity.Senha.Should().Be(senhaEsperada);
            entity.Rm.Should().Be(rmEsperado);
        }

        [Fact]
        public void ProfessorMapper_ToDTO_DevePreservarTodosOsCamposVisiveis()
        {
            // Arrange
            var professor = new Professor
            {
                Id = 10,
                Nome = "Dr. Roberto Lima",
                Email = "roberto.lima@universidade.com",
                Rm = "RM555444",
                Cpf = "44455566677",
                Senha = "naoDeveAparecerNoDTO",
                Status = StatusUsuario.BLOQUEADO
            };

            // Act
            var dto = professor.ToDTO();

            // Assert
            dto.Id.Should().Be(professor.Id);
            dto.Nome.Should().Be(professor.Nome);
            dto.Email.Should().Be(professor.Email);
            dto.Rm.Should().Be(professor.Rm);
            dto.Cpf.Should().Be(professor.Cpf);
            dto.Status.Should().Be(professor.Status.ToString());
        }

        [Theory]
        [InlineData(StatusUsuario.ATIVO, "ATIVO")]
        [InlineData(StatusUsuario.DESATIVADO, "DESATIVADO")]
        [InlineData(StatusUsuario.BLOQUEADO, "BLOQUEADO")]
        public void ProfessorMapper_ToDTO_DeveConverterTodosOsStatus(StatusUsuario status, string statusEsperado)
        {
            // Arrange
            var professor = new Professor
            {
                Id = 1,
                Nome = "Prof. Teste",
                Email = "teste@email.com",
                Rm = "RM123",
                Cpf = "12345678900",
                Senha = "hash",
                Status = status
            };

            // Act
            var dto = professor.ToDTO();

            // Assert
            dto.Status.Should().Be(statusEsperado);
        }

        [Fact]
        public void ProfessorMapper_ConversaoCompleta_DeveManterDadosBasicos()
        {
            // Arrange
            var dtoRequest = new ProfessorDTORequest
            {
                Nome = "Prof. Integração",
                Email = "integracao@universidade.com",
                Cpf = "11111111111",
                Status = "ATIVO"
            };

            // Act
            var entity = dtoRequest.ToEntity("senhaHash", "RM123456");
            entity.Id = 50;
            var dtoResponse = entity.ToDTO();

            // Assert
            dtoResponse.Nome.Should().Be(dtoRequest.Nome);
            dtoResponse.Email.Should().Be(dtoRequest.Email);
            dtoResponse.Cpf.Should().Be(dtoRequest.Cpf);
            dtoResponse.Rm.Should().Be("RM123456");
            dtoResponse.Status.Should().Be("ATIVO");
        }
    }
}
