using tivitApi.Data;
using tivitApi.Models;
using tivitApi.DTOs;
using tivitApi.Infra.SQS;
using tivitApi.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;

namespace tivitApi.Services
{
    public interface IEventoService
    {
        Task<object> criarEvento(EventoDTO eventoDTO);
        Task<EventoDTO> obterProximoEvento();
        Task<List<EventoDTO>> getAllEvents();
        Task<int> getNextWeekEvents();
    }

    public class EventoService : IEventoService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<EventoService> _logger;

        public EventoService(AppDbContext context, ILogger<EventoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private EventoDTO ConvertEventoToEventoDTO(Evento evento)
        {
            return new EventoDTO(
                evento.Id,
                evento.Titulo,
                evento.Descricao,
                evento.Horario
                );
        }


        public async Task<object> criarEvento(EventoDTO eventoDTO)
        {
            try
            {
                // ===== Validações =====
                if (eventoDTO == null)
                    throw new Exception("Dados do evento não enviados.");

                if (string.IsNullOrWhiteSpace(eventoDTO.Titulo))
                    throw new Exception("O título do evento é obrigatório.");

                if (eventoDTO.Horario == default)
                    throw new Exception("O horário do evento é inválido.");

                var evento = new Evento
                {
                    Titulo = eventoDTO.Titulo.Trim(),
                    Descricao = eventoDTO.Descricao?.Trim(),
                    Horario = eventoDTO.Horario
                };

                _context.Eventos.Add(evento);
                await _context.SaveChangesAsync();

                return new
                {
                    eventoId = evento.Id,
                    mensagem = "Evento criado com sucesso."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar evento no calendário");
                throw new Exception("Erro interno ao criar evento.");
            }
        }

        public async Task<EventoDTO> obterProximoEvento()
        {

            try
            {
                var agora = DateTime.Now;

                var proximoEvento = await _context.Eventos
                    .Where(e => e.Horario > agora)
                    .OrderBy(e => e.Horario)
                    .FirstOrDefaultAsync();

                if (proximoEvento == null)
                    throw new BusinessException("Nenhum evento futuro encontrado.");

                
                return new EventoDTO
                {
                    Titulo = proximoEvento.Titulo,
                    Descricao = proximoEvento.Descricao,
                    Horario = proximoEvento.Horario
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar próximo evento no calendário");
                throw new Exception("Erro interno ao buscar evento.");
            }


        }

        public async Task<List<EventoDTO>> getAllEvents()
        {
            var eventos = await _context.Eventos.ToListAsync();

            var resultado = eventos
            .Select(evento => ConvertEventoToEventoDTO(evento))
            .ToList();

            return resultado;
        }

        public async Task<int> getNextWeekEvents()
        {
            try
            {
                var agora = DateTime.Now;
                var limite = agora.AddDays(7);

                var quantidade = await _context.Eventos
                    .CountAsync(e => e.Horario >= agora && e.Horario <= limite);

                return quantidade;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar eventos dos próximos 7 dias");
                throw new Exception("Erro interno ao buscar quantidade de eventos.");
            }
        }

        
    }
}