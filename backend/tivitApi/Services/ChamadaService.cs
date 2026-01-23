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
        Task AtualizarChamada(List<ChamadaDTO> dtos);

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
                DateTime.UtcNow,
                dto.TurmaId
                );
        }


        public async Task RealizarChamada(List<ChamadaDTO> dtos)
        {
            if (!dtos.Any())
                return;

            var turmaId = dtos.First().TurmaId;
            var materiaId = dtos.First().MateriaId;

            var hoje = DateTime.UtcNow.Date;
            var amanha = hoje.AddDays(1);

            var chamadaJaExiste = await _context.Chamadas.AnyAsync(c =>
                c.TurmaId == turmaId &&
                c.MateriaId == materiaId &&
                c.HorarioDaAula >= hoje &&
                c.HorarioDaAula < amanha
            );

            if (chamadaJaExiste)
                throw new BusinessException("Chamada já feita para essa matéria dessa turma.");

            var chamadas = dtos.Select(dto => ConvertChamadaDtoToChamada(dto)).ToList();

            _context.Chamadas.AddRange(chamadas);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarChamada(List<ChamadaDTO> dtos)
        {
            if (!dtos.Any())
                return;

            var turmaId = dtos.First().TurmaId;
            var materiaId = dtos.First().MateriaId;

            var hoje = DateTime.UtcNow.Date;
            var amanha = hoje.AddDays(1);

            var chamadasExistentes = await _context.Chamadas
                .Where(c =>
                    c.TurmaId == turmaId &&
                    c.MateriaId == materiaId &&
                    c.HorarioDaAula >= hoje &&
                    c.HorarioDaAula < amanha
                )
                .ToListAsync();

            if (!chamadasExistentes.Any())
                throw new BusinessException("Nenhuma chamada encontrada para atualizar.");

            foreach (var dto in dtos)
            {
                var chamadaExistente = chamadasExistentes.FirstOrDefault(c =>
                    c.MatriculaId == dto.MatriculaId
                );

                if (chamadaExistente == null)
                    continue;

                var chamadaAtualizada = ConvertChamadaDtoToChamada(dto);

                chamadaExistente.Faltou = chamadaAtualizada.Faltou;
                chamadaExistente.HorarioDaAula = DateTime.UtcNow;
                
            }

            await _context.SaveChangesAsync();
        }

    }
}