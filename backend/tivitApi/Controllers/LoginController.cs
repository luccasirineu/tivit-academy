using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Services;

namespace tivitApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class LoginController : ControllerBase
	{
		private readonly ILoginService _loginService;

		public LoginController(ILoginService loginService)
		{
			_loginService = loginService;
		}

		[HttpPost]
		public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
		{
			var resultado = await _loginService.LoginAsync(loginDTO);

			if (resultado is string erro)
				return BadRequest(new { sucesso = false, mensagem = erro });

			return Ok(resultado);
		}
	}
}