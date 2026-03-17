using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using tivitApi.DTOs;
using tivitApi.Services;
using tivitApi.Models;

namespace tivitApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TurmaController : ControllerBase
    {
        private readonly ITurmaService _turmaService;
        private readonly ILogger<TurmaController> _logger;

        public TurmaController(ITurmaService turmaService, ILogger<TurmaController> logger)
        {
            _turmaService = turmaService;
            _logger = logger;
        }

        [Authorize(Roles = "administrador")]
        [HttpPost("criarTurma")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CriarTurma([FromBody] TurmaDTORequest turmaDTO, CancellationToken cancellationToken)
        {
            if (turmaDTO == null)
                return BadRequest(new { message = "Payload inválido." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _turmaService.CriarTurma(turmaDTO);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Dados inválidos ao criar turma.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar turma.");
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        // professor OR administrador
        [Authorize(Roles = "professor,administrador")]
        [HttpGet("getTurmasByCursoId/{cursoId}")]
        [ProducesResponseType(typeof(List<TurmaDTOResponse>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTurmasByCursoId(int cursoId, CancellationToken cancellationToken)
        {
            if (cursoId <= 0)
                return BadRequest(new { message = "cursoId inválido." });

            try
            {
                var turmas = await _turmaService.GetTurmasByCursoId(cursoId);
                return Ok(turmas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter turmas do curso {CursoId}", cursoId);
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        [HttpGet("aluno/{alunoId}/turma")]
        [ProducesResponseType(typeof(TurmaDTOResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTurmaByAlunoId(int alunoId, CancellationToken cancellationToken)
        {
            if (alunoId <= 0)
                return BadRequest(new { message = "alunoId inválido." });

            try
            {
                var turma = await _turmaService.GetTurmaByAlunoId(alunoId);
                if (turma == null)
                    return NotFound(new { message = "Turma não encontrada para o aluno." });

                return Ok(turma);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter turma do aluno {AlunoId}", alunoId);
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        [Authorize(Roles = "professor,administrador")]
        [HttpGet("getQntdTurmasAtivas")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetQntdTurmasAtivas(CancellationToken cancellationToken)
        {
            try
            {
                var qntdTurmasAtivas = await _turmaService.GetQntdTurmasAtivas();
                return Ok(qntdTurmasAtivas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter quantidade de turmas ativas.");
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        [Authorize(Roles = "administrador")]
        [HttpGet("getAllTurmas")]
        [ProducesResponseType(typeof(List<TurmaDTOResponse>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllTurmas(CancellationToken cancellationToken)
        {
            try
            {
                var turmas = await _turmaService.GetAllTurmas();
                return Ok(turmas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar turmas.");
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        [Authorize(Roles = "administrador")]
        [HttpPut("atualizarTurma")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AtualizarTurma([FromBody] TurmaDTORequest dto, CancellationToken cancellationToken)
        {
            if (dto == null)
                return BadRequest(new { message = "Payload inválido." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _turmaService.AtualizarTurma(dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Turma não encontrada ao atualizar. Id: {Id}", dto?.Id);
                return NotFound(new { message = "Turma não encontrada." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Dados inválidos ao atualizar turma.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar turma Id:{Id}", dto?.Id);
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }
    }
}