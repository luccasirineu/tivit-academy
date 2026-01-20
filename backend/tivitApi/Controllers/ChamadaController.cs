using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Services;

namespace tivitApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChamadaController : ControllerBase
    {
        private readonly IChamadaService _chamadaService;

        public ChamadaController(IChamadaService chamadaService)
        {
            _chamadaService = chamadaService;
        }

        [HttpPost("realizarChamada")]
        public async Task<IActionResult> RealizarChamada([FromBody] List<ChamadaDTO> dtos)
        {

            try
            {
                await _chamadaService.RealizarChamada(dtos);

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
