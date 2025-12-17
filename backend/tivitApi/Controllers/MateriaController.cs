using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Services;
using tivitApi.Models;


namespace tivitApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MateriaController : ControllerBase
    {
        private readonly IMateriaService _materiaService;

        public MateriaController(IMateriaService materiaService)
        {
            _materiaService = materiaService;
        }


        [HttpPost]
        public async Task<IActionResult> CriarMateria([FromBody] MateriaDTO dto)
        {
            var materia = await _materiaService.CriarMateriaAsync(dto);

            return CreatedAtAction(
                nameof(CriarMateria),
                new { id = materia.Id },
                materia
            );
        }
    }
}
