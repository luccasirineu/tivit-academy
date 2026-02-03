using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Services;

namespace tivitApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlunoController : ControllerBase
    {
        private readonly IAlunoService _alunoService;

        public AlunoController(IAlunoService alunoService)
        {
            _alunoService = alunoService;
        }


        [HttpGet("contextMe/{alunoId}")]
        public async Task<IActionResult> GetInfoAluno(int alunoId)
        {
           
            try
            {
                var infoAluno = await _alunoService.GetInfoAluno(alunoId);
                return Ok(infoAluno);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("getAllAlunosByCurso/{cursoId}")]
        public async Task<IActionResult> GetAllAlunosByCurso(int cursoId)
        {

            try
            {
                var alunosByCurso = await _alunoService.GetAllAlunosByCurso(cursoId);
                return Ok(alunosByCurso);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("getAllAlunosByTurmaId/{turmaId}")]
        public async Task<IActionResult> GetAllAlunosByTurmaId(int turmaId)
        {

            try
            {
                var alunosByTurma = await _alunoService.GetAllAlunosByTurmaId(turmaId);
                return Ok(alunosByTurma);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("getAlunoByMatriculaId/{matriculaId}")]
        public async Task<IActionResult> GetAlunoByMatriculaId(int matriculaId)
        {

            try
            {
                var aluno = await _alunoService.GetAlunoByMatriculaId(matriculaId);
                return Ok(aluno);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("getQntdAlunosAtivos")]
        public async Task<IActionResult> GetQntdAlunosAtivos()
        {

            try
            {
                var qntdAlunosAtivos = await _alunoService.GetQntdAlunosAtivos();
                return Ok(qntdAlunosAtivos);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}
