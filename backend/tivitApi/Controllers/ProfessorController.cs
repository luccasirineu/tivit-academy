using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using tivitApi.DTOs;
using tivitApi.Services;

namespace tivitApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfessorController : ControllerBase
    {
        private readonly IProfessorService _professorService;
        private readonly ILogger<ProfessorController> _logger;

        public ProfessorController(IProfessorService professorService, ILogger<ProfessorController> logger)
        {
            _professorService = professorService;
            _logger = logger;
        }

        [Authorize(Roles = "administrador")]
        [HttpGet("getQntdProfessoresAtivos")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetQntdProfessoresAtivos(CancellationToken cancellationToken)
        {
            try
            {
                var qntd = await _professorService.GetQntdProfessoresAtivos();
                return Ok(qntd);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter quantidade de professores ativos");
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        [Authorize(Roles = "professor,administrador")]
        [HttpGet("getProfessorById/{professorId}")]
        [ProducesResponseType(typeof(ProfessorDTOResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProfessorById(int professorId, CancellationToken cancellationToken)
        {
            if (professorId <= 0)
                return BadRequest(new { message = "professorId inválido." });

            try
            {
                var professor = await _professorService.GetProfessorById(professorId);
                if (professor == null)
                    return NotFound(new { message = "Professor não encontrado." });

                return Ok(professor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter professor {ProfessorId}", professorId);
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        [Authorize(Roles = "administrador")]
        [HttpGet("getAllProfessores")]
        [ProducesResponseType(typeof(List<ProfessorDTOResponse>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllProfessores(CancellationToken cancellationToken)
        {
            try
            {
                var professores = await _professorService.GetAllProfessores();
                return Ok(professores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar professores.");
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        [Authorize(Roles = "administrador")]
        [HttpGet("getAllProfessoresAtivos")]
        [ProducesResponseType(typeof(List<ProfessorDTOResponse>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllProfessoresAtivos(CancellationToken cancellationToken)
        {
            try
            {
                var professores = await _professorService.GetAllProfessoresAtivos();
                return Ok(professores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar professores ativos.");
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        [Authorize(Roles = "administrador")]
        [HttpPost("criarProfessor")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CriarProfessor([FromBody] ProfessorDTORequest professorDTO, CancellationToken cancellationToken)
        {
            if (professorDTO == null)
                return BadRequest(new { message = "Payload inválido." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _professorService.CriarProfessor(professorDTO);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Dados inválidos ao criar professor.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar professor.");
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }
    }
}