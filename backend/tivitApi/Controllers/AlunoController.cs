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
                var infoAluno = await _alunoService.getInfoAluno(alunoId);
                return Ok(infoAluno);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}
