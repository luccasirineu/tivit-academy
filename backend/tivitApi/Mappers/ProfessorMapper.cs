using tivitApi.DTOs;
using tivitApi.Models;

namespace tivitApi.Mappers
{
    /// <summary>
    /// Mapper para conversões entre Professor e ProfessorDTO
    /// </summary>
    public static class ProfessorMapper
    {
        public static ProfessorDTOResponse ToDTO(this Professor professor)
        {
            return new ProfessorDTOResponse(
                professor.Id,
                professor.Nome,
                professor.Email,
                professor.Rm,
                professor.Cpf,
                professor.Status
            );
        }

        public static Professor ToEntity(this ProfessorDTORequest dto, string senha, string rm)
        {
            return new Professor
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Cpf = dto.Cpf,
                Rm = rm,
                Senha = senha,
                Status = dto.Status
            };
        }
    }
}
