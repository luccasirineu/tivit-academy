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
    public class AlunoController : ControllerBase
    {
        private readonly IAlunoService _alunoService;
        private readonly ILogger<AlunoController> _logger;

        public AlunoController(IAlunoService alunoService, ILogger<AlunoController> logger)
        {
            _alunoService = alunoService;
            _logger = logger;
        }

        [HttpGet("contextMe/{alunoId}")]
        [ProducesResponseType(typeof(AlunoDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetInfoAluno(int alunoId)
        {
            try
            {
                var infoAluno = await _alunoService.GetInfoAluno(alunoId);
                return Ok(infoAluno);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aluno não encontrado: {AlunoId}", alunoId);
                return NotFound(new { message = "Aluno não encontrado." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter informações do aluno {AlunoId}", alunoId);
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        [Authorize(Roles = "professor,administrador")]
        [HttpGet("getAllAlunosByCurso/{cursoId}")]
        [ProducesResponseType(typeof(List<AlunoDTO>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAlunosByCurso(int cursoId)
        {
            try
            {
                var alunosByCurso = await _alunoService.GetAllAlunosByCurso(cursoId);
                return Ok(alunosByCurso);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Curso não encontrado: {CursoId}", cursoId);
                return NotFound(new { message = "Curso não encontrado." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar alunos do curso {CursoId}", cursoId);
                return Problem(statusCode: 500, detail: "Erro interno ao processar a requisição.");
            }
        }

        [Authorize(Roles = "professor,administrador")]
        [HttpGet("getAllAlunosByTurmaId/{turmaId}")]
        [ProducesResponseType(typeof(List<AlunoDTO>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAlunosByTurmaId(int turmaId)
        {
            try
            {
                var alunosByTurma = await _alunoService.GetAllAlunosByTurmaId(turmaId);
                return Ok(alunosByTurma);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Turma não encontrada: {TurmaId}", turmaId);
                return NotFound(new { message = "Turma não encontrada." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar alunos da turma {TurmaId}", turmaId);
                return Problem(statusCode: 500, detail: "Erro interno ao processar a requisição.");
            }
        }

        [Authorize(Roles = "professor,administrador")]
        [HttpGet("matricula/{matriculaId}")]
        [ProducesResponseType(typeof(AlunoDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAlunoByMatriculaId(int matriculaId)
        {
            try
            {
                var aluno = await _alunoService.GetAlunoByMatriculaId(matriculaId);
                return Ok(aluno);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Matrícula não encontrada: {MatriculaId}", matriculaId);
                return NotFound(new { message = "Matrícula não encontrada." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter aluno por matrícula {MatriculaId}", matriculaId);
                return Problem(statusCode: 500, detail: "Erro interno ao processar a requisição.");
            }
        }

        [Authorize(Roles = "professor,administrador")]
        [HttpGet("getQntdAlunosAtivos")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetQntdAlunosAtivos()
        {
            try
            {
                var qntdAlunosAtivos = await _alunoService.GetQntdAlunosAtivos();
                return Ok(qntdAlunosAtivos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter quantidade de alunos ativos");
                return Problem(statusCode: 500, detail: "Erro interno ao processar a requisição.");
            }
        }

        [Authorize(Roles = "professor,administrador")]
        [HttpGet("getAllAlunos")]
        [ProducesResponseType(typeof(List<AlunoDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAlunos()
        {
            try
            {
                var alunos = await _alunoService.GetAllAlunos();
                return Ok(alunos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar todos os alunos");
                return Problem(statusCode: 500, detail: "Erro interno ao processar a requisição.");
            }
        }

        [Authorize(Roles = "administrador")]
        [HttpPatch("{id}/turma")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateTurmaAluno(int id, [FromBody] UpdateTurmaAlunoDTO dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Payload inválido." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _alunoService.UpdateTurmaAluno(id, dto.TurmaId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aluno ou Turma não encontrado ao atualizar turma do aluno {AlunoId}", id);
                return NotFound(new { message = "Aluno ou Turma não encontrado." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Requisição inválida ao atualizar turma do aluno {AlunoId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar turma do aluno {AlunoId}", id);
                return Problem(statusCode: 500, detail: "Erro interno ao processar a requisição.");
            }
        }

        [HttpPatch("recuperarSenha/{cpf}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RecuperarSenha(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return BadRequest(new { message = "CPF é obrigatório." });

            try
            {
                await _alunoService.ResetSenha(cpf);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "CPF inválido ao recuperar senha {Cpf}", cpf);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar senha do CPF {Cpf}", cpf);
                return Problem(statusCode: 500, detail: "Erro interno ao processar a requisição.");
            }
        }
    }
}