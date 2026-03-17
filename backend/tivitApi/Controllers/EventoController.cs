using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace tivitApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EventoController : ControllerBase
    {
        private readonly IEventoService _eventoService;
        private readonly ILogger<EventoController> _logger;

        public EventoController(IEventoService eventoService, ILogger<EventoController> logger)
        {
            _eventoService = eventoService;
            _logger = logger;
        }

        [Authorize(Roles = "professor")]
        [HttpPost]
        [ProducesResponseType(typeof(EventoDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AdicionarEvento([FromBody] EventoDTO eventoDTO, CancellationToken cancellationToken)
        {
            if (eventoDTO == null)
                return BadRequest(new { message = "Payload inválido." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var resultado = await _eventoService.criarEvento(eventoDTO);
                // serviço atual retorna object: trate tipos esperados (preferir alterar o service para retornar EventoDTO/Result)
                if (resultado is string erro)
                    return BadRequest(new { sucesso = false, mensagem = erro });

                if (resultado is EventoDTO created && created.Id > 0)
                    return CreatedAtAction(nameof(GetAllEvents), new { id = created.Id }, created);

                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Dados inválidos ao criar evento.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar evento.");
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        // professor OR aluno
        [Authorize(Roles = "professor,aluno")]
        [HttpGet("proximoEvento")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ProximoEvento(CancellationToken cancellationToken)
        {
            try
            {
                var eventoDTO = await _eventoService.obterProximoEvento();
                if (eventoDTO == null)
                    return NoContent();

                return Ok(new
                {
                    eventoDTO.Titulo,
                    eventoDTO.Descricao,
                    eventoDTO.Horario
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter próximo evento.");
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        [Authorize(Roles = "professor,aluno")]
        [HttpGet("getAllEvents")]
        [ProducesResponseType(typeof(System.Collections.Generic.List<EventoDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllEvents(CancellationToken cancellationToken)
        {
            try
            {
                var eventosDTO = await _eventoService.getAllEvents();
                return Ok(eventosDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter eventos.");
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        [Authorize(Roles = "professor,aluno")]
        [HttpGet("getNextWeekEvents")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetNextWeekEvents(CancellationToken cancellationToken)
        {
            try
            {
                var qntdEventos = await _eventoService.getNextWeekEvents();
                return Ok(qntdEventos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter eventos da próxima semana.");
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }
    }
}