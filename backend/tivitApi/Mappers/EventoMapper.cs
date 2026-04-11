using tivitApi.DTOs;
using tivitApi.Models;

namespace tivitApi.Mappers
{
    /// <summary>
    /// Mapper para conversões entre Evento e EventoDTO
    /// </summary>
    public static class EventoMapper
    {
        public static EventoDTO ToDTO(this Evento evento)
        {
            return new EventoDTO(
                evento.Id,
                evento.Titulo,
                evento.Descricao,
                evento.Horario
            );
        }
    }
}
