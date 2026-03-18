using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Services;
using tivitApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace tivitApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MateriaController : ControllerBase
    {
        private readonly IMateriaService _materiaService;
        private readonly ILogger<MateriaController> _logger;

        public MateriaController(IMateriaService materiaService, ILogger<MateriaController> logger)
        {
            _materiaService = materiaService;
            _logger = logger;
        }

        [Authorize(Roles = "administrador")]
        [HttpPost("criarMateria")]
        [ProducesResponseType(typeof(Materia), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CriarMateria([FromBody] MateriaDTO dto, CancellationToken cancellationToken)
        {
            if (dto == null)
                return BadRequest(new { message = "Payload inválido." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var materia = await _materiaService.CriarMateriaAsync(dto);
                // Preferível: criar um GET /api/materia/{id} e usar CreatedAtAction apontando para ele.
                return Created($"/api/materia/{materia.Id}", materia);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Dados inválidos ao criar matéria.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar matéria.");
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        [Authorize(Roles = "professor,administrador,aluno")]
        [HttpGet("getMateriasByCursoId/{cursoId}")]
        [ProducesResponseType(typeof(List<Materia>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetMateriasByCursoId(int cursoId, CancellationToken cancellationToken)
        {
            if (cursoId <= 0)
                return BadRequest(new { message = "ID do curso inválido." });

            try
            {
                var materias = await _materiaService.GetMateriasByCursoIdAsync(cursoId);
                if (materias == null || materias.Count == 0)
                    return NotFound(new { message = "Nenhuma matéria encontrada para este curso." });

                return Ok(materias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter matérias do curso {CursoId}", cursoId);
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        [HttpGet("getCursoId/{alunoId}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCursoId(int alunoId, CancellationToken cancellationToken)
        {
            if (alunoId <= 0)
                return BadRequest(new { message = "ID do aluno inválido." });

            try
            {
                var cursoId = await _materiaService.GetCursoIdByAlunoIdAsync(alunoId);
                return Ok(new { cursoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter cursoId para aluno {AlunoId}", alunoId);
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        [HttpGet("getNomeMateria/{materiaId}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetMateriaNome(int materiaId, CancellationToken cancellationToken)
        {
            if (materiaId <= 0)
                return BadRequest(new { message = "ID da matéria inválido." });

            try
            {
                var materiaNome = await _materiaService.GetMateriaNomeByMateriaIdAsync(materiaId);
                if (string.IsNullOrEmpty(materiaNome))
                    return NotFound(new { message = "Matéria não encontrada." });

                return Ok(new { materiaNome });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter nome da matéria {MateriaId}", materiaId);
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }
    }
}