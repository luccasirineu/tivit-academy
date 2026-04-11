using tivitApi.DTOs;
using tivitApi.Models;

namespace tivitApi.Mappers
{
    /// <summary>
    /// Mapper para conversões entre Curso e CursoDTO
    /// </summary>
    public static class CursoMapper
    {
        public static CursoDTO ToDTO(this Curso curso)
        {
            return new CursoDTO(
                curso.Id,
                curso.Nome,
                curso.Descricao,
                curso.ProfResponsavel,
                curso.Status
            );
        }

        public static Curso ToEntity(this CursoDTORequest dto)
        {
            return new Curso(
                dto.Nome,
                dto.Descricao,
                dto.ProfResponsavel,
                "ATIVO"
            );
        }
    }
}
