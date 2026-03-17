using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace tivitApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacaoController : ControllerBase
    {
        private readonly INotificacaoService _notificacaoService;
        private readonly ILogger<NotificacaoController> _logger;

        public NotificacaoController(INotificacaoService notificacaoService, ILogger<NotificacaoController> logger)
        {
            _notificacaoService = notificacaoService;
            _logger = logger;
        }

        [Authorize(Roles = "administrador")]
        [HttpPost("criarNotificacao")]
        [ProducesResponseType(typeof(object), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CriarNotificacao([FromBody] NotificacaoDTORequest notificacaoDTO, CancellationToken cancellationToken)
        {
            if (notificacaoDTO == null)
                return BadRequest(new { message = "Payload inválido." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (notificacaoDTO.TurmasIds == null || notificacaoDTO.TurmasIds.Count == 0)
                return BadRequest(new { message = "Ao menos uma turma deve ser informada." });

            try
            {
                await _notificacaoService.CriarNotificacao(notificacaoDTO);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Dados inválidos ao criar notificação.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar notificação.");
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        [Authorize(Roles = "aluno")]
        [HttpGet("getNotificacoesByTurmaId/{turmaId}")]
        [ProducesResponseType(typeof(List<NotificacaoDTOResponse>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetNotificacaoByTurmaId(int turmaId, CancellationToken cancellationToken)
        {
            if (turmaId <= 0)
                return BadRequest(new { message = "turmaId inválido." });

            try
            {
                // Recomenda-se validar no service se o aluno pertence à turma (claims)
                var notificacoes = await _notificacaoService.GetNotificacoesByTurmaId(turmaId);
                return Ok(notificacoes ?? new List<NotificacaoDTOResponse>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter notificações da turma {TurmaId}", turmaId);
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }
    }
}