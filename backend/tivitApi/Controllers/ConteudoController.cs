using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Services;

namespace tivitApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ConteudoController : ControllerBase
    {
        private readonly IConteudoService _conteudoService;
        private readonly ILogger<ConteudoController> _logger;

        public ConteudoController(IConteudoService conteudoService, ILogger<ConteudoController> logger)
        {
            _conteudoService = conteudoService;
            _logger = logger;
        }

        /// <summary>
        /// Faz o upload de um novo conteúdo em formato PDF.
        /// </summary>
        [Authorize(Roles = "professor")]
        [HttpPost("upload/pdf")]
        [ProducesResponseType(typeof(Conteudo), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadPdf([FromForm] CreateConteudoPdfDTO dto, CancellationToken cancellationToken)
        {
            if (dto == null)
                return BadRequest(new { message = "Payload inválido." });

            if (dto.Arquivo == null || dto.Arquivo.Length == 0)
                return BadRequest(new { message = "Arquivo é obrigatório." });

            // simples validação de tipo/ extensão
            if (!string.Equals(dto.Arquivo.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase)
                && !dto.Arquivo.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Apenas arquivos PDF são permitidos." });
            }

            // obter professorId do token (ajuste conforme claim usado no seu Auth)
            var claimValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("id")?.Value;
            if (!int.TryParse(claimValue, out var professorId))
                return Forbid();

            try
            {
                var conteudo = await _conteudoService.CriarConteudoPdfAsync(dto, professorId);
                return Created($"/api/conteudo/{conteudo.Id}", conteudo);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Requisição inválida ao criar conteúdo PDF.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar conteúdo PDF.");
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        /// <summary>
        /// Cria um novo conteúdo a partir de um link externo.
        /// </summary>
        [Authorize(Roles = "professor")]
        [HttpPost("upload/link")]
        [ProducesResponseType(typeof(Conteudo), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CriarLink([FromBody] CreateConteudoLinkDTO dto, CancellationToken cancellationToken)
        {
            if (dto == null)
                return BadRequest(new { message = "Payload inválido." });

            if (string.IsNullOrWhiteSpace(dto.Url) || !Uri.TryCreate(dto.Url, UriKind.Absolute, out var uri)
                || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return BadRequest(new { message = "URL inválida." });
            }

            var claimValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("id")?.Value;
            if (!int.TryParse(claimValue, out var professorId))
                return Forbid();

            try
            {
                var conteudo = await _conteudoService.CriarConteudoLinkAsync(dto, professorId);
                return Created($"/api/conteudo/{conteudo.Id}", conteudo);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Requisição inválida ao criar conteúdo link.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar conteúdo link.");
                return Problem(detail: "Erro interno ao processar a requisi��o.", statusCode: 500);
            }
        }

        /// <summary>
        /// Obtém a lista de conteúdos vinculados a uma matéria e turma.
        /// </summary>
        [Authorize(Roles = "aluno")]
        [HttpGet("getAllConteudos/{materiaId:int}/{turmaId:int}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetConteudo(int materiaId, int turmaId, CancellationToken cancellationToken)
        {
            if (materiaId <= 0 || turmaId <= 0)
                return BadRequest(new { message = "Parâmetros inválidos." });

            try
            {
                var conteudos = await _conteudoService.GetConteudosByMateriaIdAsync(materiaId, turmaId);
                return Ok(new { conteudos });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter conteúdos.");
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }
    }
}