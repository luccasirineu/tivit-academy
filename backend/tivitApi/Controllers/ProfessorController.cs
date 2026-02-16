using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Services;

namespace tivitApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfessorController : ControllerBase
    {
        private readonly IProfessorService _professorService;

        public ProfessorController(IProfessorService professorService)
        {
            _professorService = professorService;
        }
         

        [HttpGet("getQntdProfessoresAtivos")]
        public async Task<IActionResult> GetQntdAlunosAtivos()
        {

            try
            {
                var qntdProfessoresAtivos = await _professorService.GetQntdProfessoresAtivos();
                return Ok(qntdProfessoresAtivos);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpGet("getProfessorById/{professorId}")]
        public async Task<IActionResult> GetProfessorById(int professorId)
        {

            try
            {
                var professor = await _professorService.GetProfessorById(professorId);
                return Ok(professor);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("getAllProfessores")]
        public async Task<IActionResult> GetAllProfessores()
        {
            try
            {
                var professores = await _professorService.GetAllProfessores();
                return Ok(professores);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("getAllProfessoresAtivos")]
        public async Task<IActionResult> GetAllProfessoresAtivos()
        {
            try
            {
                var professores = await _professorService.GetAllProfessoresAtivos();
                return Ok(professores);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("criarProfessor")]
        public async Task<IActionResult> CriarProfessor([FromBody] ProfessorDTORequest professorDTO)
        {
            try
            {
                await _professorService.CriarProfessor(professorDTO);

                return NoContent();

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    erro = ex.Message
                });
            }
        }
    }
}
