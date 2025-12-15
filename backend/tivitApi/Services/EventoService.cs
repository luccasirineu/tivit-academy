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

                // ===== Retorno =====
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


    }
}