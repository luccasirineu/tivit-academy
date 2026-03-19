using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using tivitApi.DTOs;
using tivitApi.Services;

namespace tivitApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatConteudoController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatConteudoController> _logger;

        public ChatConteudoController(
            IChatService chatService,
            ILogger<ChatConteudoController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        /// <summary>
        /// Responde uma pergunta sobre um conteúdo específico usando IA
        /// </summary>
        /// <param name="conteudoId">ID do conteúdo</param>
        /// <param name="dto">Pergunta do aluno</param>
        /// <returns>Resposta baseada no contexto do conteúdo</returns>
        [HttpPost("{conteudoId}/perguntar")]
        [ProducesResponseType(typeof(ChatRespostaDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResponderPergunta(
            [FromRoute] int conteudoId,
            [FromBody] ChatPerguntaDTO dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Pergunta))
                {
                    return BadRequest(new { message = "Pergunta é obrigatória" });
                }

                // Obter aluno ID do token
                var alunoIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (alunoIdClaim == null || !int.TryParse(alunoIdClaim.Value, out var alunoId))
                {
                    return Unauthorized(new { message = "Aluno não identificado" });
                }

               

                _logger.LogInformation($"Aluno {alunoId} fez pergunta sobre conteúdo {conteudoId}");

                var resposta = await _chatService.ResponderPerguntaAsync(
                    conteudoId,
                    alunoId,
                    dto.Pergunta);

                if (!resposta.Sucesso)
                {
                    return BadRequest(resposta);
                }

                return Ok(resposta);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao responder pergunta: {ex.Message}");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "Erro ao processar pergunta", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém o contexto de um conteúdo (para debug)
        /// </summary>
        [HttpGet("{conteudoId}/contexto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterContexto([FromRoute] int conteudoId)
        {
            try
            {
                var contexto = await _chatService.ObterContextoConteudoAsync(conteudoId);

                if (contexto == null)
                {
                    return NotFound(new { message = "Contexto não encontrado" });
                }

                return Ok(new
                {
                    conteudoId = contexto.ConteudoId,
                    status = contexto.StatusExtracao,
                    dataArmazenamento = contexto.DataArmazenamento,
                    temErro = !string.IsNullOrEmpty(contexto.MensagemErro),
                    mensagemErro = contexto.MensagemErro
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter contexto: {ex.Message}");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "Erro ao obter contexto" });
            }
        }
    }
}
