using tivitApi.DTOs;
using tivitApi.Models;

namespace tivitApi.Mappers
{
    /// <summary>
    /// Mapper para conversões entre Chamada e ChamadaDTO
    /// </summary>
    public static class ChamadaMapper
    {
        public static Chamada ToEntity(this ChamadaDTO dto)
        {
            return new Chamada(
                dto.MatriculaId,
                dto.MateriaId,
                dto.Faltou,
                DateTime.UtcNow,
                dto.TurmaId
            );
        }
    }
}
