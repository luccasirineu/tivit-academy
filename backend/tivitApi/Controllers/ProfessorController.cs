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

    }
}
