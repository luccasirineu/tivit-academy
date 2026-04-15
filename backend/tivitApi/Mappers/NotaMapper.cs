using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Enums;

namespace tivitApi.Mappers
{
    /// <summary>
    /// Mapper para conversões entre Nota e NotaDTO
    /// </summary>
    public static class NotaMapper
    {
        public static Nota ToEntity(this NotaDTORequest dto, decimal media, StatusNota status, int qntdFaltas)
        {
            return new Nota(
                dto.AlunoId,
                dto.MateriaId,
                dto.Nota1,
                dto.Nota2,
                media,
                qntdFaltas,
                status
            );
        }
    }
}
