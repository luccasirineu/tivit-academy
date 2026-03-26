using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace tivitApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CursoController : ControllerBase
    {
        private readonly ICursoService _cursoService;
        private readonly ILogger<CursoController> _logger;

        public CursoController(ICursoService cursoService, ILogger<CursoController> logger)
        {
            _cursoService = cursoService;
            _logger = logger;
        }

        /// <summary>
        /// Obtém a lista de todos os cursos.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<CursoDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllCursos(CancellationToken cancellationToken)
        {
            try
            {
                var cursosDTOs = await _cursoService.GetAllCursosAsync();
                return Ok(cursosDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar cursos");
                return Problem(detail: "Erro interno ao listar cursos.", statusCode: 500);
            }
        }

        /// <summary>
        /// Obtém a lista de todos os cursos ativos.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("getAllCursosAtivos")]
        [ProducesResponseType(typeof(List<CursoDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllCursosAtivos(CancellationToken cancellationToken)
        {
            try
            {
                var cursosDTOs = await _cursoService.GetAllCursosAtivos();
                return Ok(cursosDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar cursos ativos");
                return Problem(detail: "Erro interno ao listar cursos ativos.", statusCode: 500);
            }
        }

        /// <summary>
        /// Obtém os detalhes de um curso específico pelo ID.
        /// </summary>
        [Authorize(Roles = "administrador")]
        [HttpGet("{cursoId}")]
        [ProducesResponseType(typeof(CursoDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCursoId(int cursoId, CancellationToken cancellationToken)
        {
            try
            {
                var cursoDTO = await _cursoService.GetCursoById(cursoId);
                if (cursoDTO == null)
                    return NotFound(new { message = "Curso não encontrado." });

                return Ok(cursoDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter curso {CursoId}", cursoId);
                return Problem(detail: "Erro interno ao obter curso.", statusCode: 500);
            }
        }

        /// <summary>
        /// Obtém a quantidade de cursos atribuídos a um professor específico.
        /// </summary>
        [Authorize(Roles = "administrador, professor")]
        [HttpGet("getQntdCursosProf/{professorId}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetQntdCursosProf(int professorId, CancellationToken cancellationToken)
        {
            try
            {
                var qntdEventos = await _cursoService.GetQntdCursosProf(professorId);
                return Ok(qntdEventos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter quantidade de cursos do professor {ProfessorId}", professorId);
                return Problem(detail: "Erro interno.", statusCode: 500);
            }
        }

        /// <summary>
        /// Obtém a lista de cursos atribuídos a um professor específico.
        /// </summary>
        [Authorize(Roles = "administrador, professor")]
        [HttpGet("getAllCursosProf/{professorId}")]
        [ProducesResponseType(typeof(List<CursoDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllCursosProfAsync(int professorId, CancellationToken cancellationToken)
        {
            try
            {
                var cursosProfessor = await _cursoService.GetAllCursosProfAsync(professorId);
                return Ok(cursosProfessor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar cursos do professor {ProfessorId}", professorId);
                return Problem(detail: "Erro interno.", statusCode: 500);
            }
        }

        /// <summary>
        /// Obtém a quantidade de alunos matriculados em um curso específico.
        /// </summary>
        [Authorize(Roles = "administrador")]
        [HttpGet("getQntdAlunosByCursoId/{cursoId}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetQntdAlunosByCursoId(int cursoId, CancellationToken cancellationToken)
        {
            try
            {
                var qntdAlunos = await _cursoService.GetQntdAlunosByCursoId(cursoId);
                return Ok(qntdAlunos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter quantidade de alunos do curso {CursoId}", cursoId);
                return Problem(detail: "Erro interno.", statusCode: 500);
            }
        }

        /// <summary>
        /// Registra um novo curso no sistema.
        /// </summary>
        [Authorize(Roles = "administrador")]
        [HttpPost("criarCurso")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CriarCurso([FromBody] CursoDTORequest dto, CancellationToken cancellationToken)
        {
            if (dto == null)
                return BadRequest(new { message = "Payload inválido." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _cursoService.CriarCurso(dto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Dados inválidos ao criar curso.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar curso.");
                return Problem(detail: "Erro interno ao criar curso.", statusCode: 500);
            }
        }

        /// <summary>
        /// Atualiza as informações de um curso existente.
        /// </summary>
        [Authorize(Roles = "administrador")]
        [HttpPut("atualizarCurso")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AtualizarCurso([FromBody] CursoDTORequest dto, CancellationToken cancellationToken)
        {
            if (dto == null)
                return BadRequest(new { message = "Payload inválido." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _cursoService.AtualizarCurso(dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Curso não encontrado ao atualizar.");
                return NotFound(new { message = "Curso não encontrado." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Dados inválidos ao atualizar curso.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar curso.");
                return Problem(detail: "Erro interno ao atualizar curso.", statusCode: 500);
            }
        }

        /// <summary>
        /// Desativa um curso existente no sistema.
        /// </summary>
        [Authorize(Roles = "administrador")]
        [HttpPut("desativarCurso/{cursoId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DesativarCurso(int cursoId, CancellationToken cancellationToken)
        {
            try
            {
                await _cursoService.DesativarCurso(cursoId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Curso não encontrado ao desativar {CursoId}", cursoId);
                return NotFound(new { message = "Curso não encontrado." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar curso {CursoId}", cursoId);
                return Problem(detail: "Erro interno ao desativar curso.", statusCode: 500);
            }
        }

        /// <summary>
        /// Ativa um curso previamente desativado.
        /// </summary>
        [Authorize(Roles = "administrador")]
        [HttpPut("ativarCurso/{cursoId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AtivarCurso(int cursoId, CancellationToken cancellationToken)
        {
            try
            {
                await _cursoService.AtivarCurso(cursoId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Curso não encontrado ao ativar {CursoId}", cursoId);
                return NotFound(new { message = "Curso não encontrado." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao ativar curso {CursoId}", cursoId);
                return Problem(detail: "Erro interno ao ativar curso.", statusCode: 500);
            }
        }
    }
}
