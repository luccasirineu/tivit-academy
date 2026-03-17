using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Services;
using tivitApi.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Linq;

namespace tivitApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChamadaController : ControllerBase
    {
        private readonly IChamadaService _chamadaService;
        private readonly ILogger<ChamadaController> _logger;

        public ChamadaController(IChamadaService chamadaService, ILogger<ChamadaController> logger)
        {
            _chamadaService = chamadaService;
            _logger = logger;
        }

        [Authorize(Roles = "professor")]
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RealizarChamada([FromBody] List<ChamadaDTO> dtos, CancellationToken cancellationToken)
        {
            if (dtos == null || !dtos.Any())
                return BadRequest(new { message = "Lista de chamadas não pode ser vazia." });

            if (dtos.Any(d => d.MatriculaId <= 0 || d.MateriaId <= 0 || d.TurmaId <= 0))
                return BadRequest(new { message = "Cada chamada deve conter MatriculaId, MateriaId e TurmaId válidos." });

            try
            {
                await _chamadaService.RealizarChamada(dtos);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Requisição inválida ao realizar chamada.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao realizar chamada.");
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        [Authorize(Roles = "professor")]
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AtualizarChamada([FromBody] List<ChamadaDTO> dtos, CancellationToken cancellationToken)
        {
            if (dtos == null || !dtos.Any())
                return BadRequest(new { message = "Lista de chamadas não pode ser vazia." });

            if (dtos.Any(d => d.MatriculaId <= 0 || d.MateriaId <= 0 || d.TurmaId <= 0))
                return BadRequest(new { message = "Cada chamada deve conter MatriculaId, MateriaId e TurmaId válidos." });

            try
            {
                await _chamadaService.AtualizarChamada(dtos);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Requisição inválida ao atualizar chamada.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao atualizar chamada.");
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }
    }
}