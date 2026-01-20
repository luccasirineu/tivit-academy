using Microsoft.EntityFrameworkCore;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Exceptions;

namespace tivitApi.Services
{
    public interface IChamadaService
    {
        Task RealizarChamada(List<ChamadaDTO> dtos);
    }

    public class ChamadaService : IChamadaService
    {
        private readonly AppDbContext _context;

        public ChamadaService(AppDbContext context)
        {
            _context = context;

        }


        private Chamada ConvertChamadaDtoToChamada(ChamadaDTO dto)
        {
            return new Chamada(
                dto.MatriculaId,
                dto.MateriaId,
                dto.Faltou,
                DateTime.UtcNow
                );
        }


        public async Task RealizarChamada(List<ChamadaDTO> dtos)
        {
            var chamadas = dtos.Select(dto => ConvertChamadaDtoToChamada(dto)).ToList();

            _context.Chamadas.AddRange(chamadas);
            await _context.SaveChangesAsync();
        }

    }
}