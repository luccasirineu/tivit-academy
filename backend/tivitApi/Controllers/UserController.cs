using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace tivitApi.Controllers
{
    [Authorize(Roles = "administrador")]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Obtém as informações de um usuário (aluno, professor ou admin) através do CPF.
        /// </summary>
        [HttpGet("getUserByCpf")]
        public async Task<IActionResult> GetUserByCpf([FromQuery] string cpf, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return BadRequest(new { message = "CPF é obrigatório." });

            try
            {
                var user = await _userService.GetUserByCpf(cpf);
                if (user == null)
                    return NotFound(new { message = "Usuário não encontrado." });

                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Requisição inválida ao buscar usuário por CPF {Cpf}", MaskCpf(cpf));
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário por CPF {Cpf}", MaskCpf(cpf));
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        /// <summary>
        /// Realiza a busca de usuários através do nome.
        /// </summary>
        [HttpGet("getUsersByNome")]
        public async Task<IActionResult> GetUsersByNome([FromQuery] string nome, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return BadRequest(new { message = "Nome do usuário é obrigatório." });

            try
            {
                var users = await _userService.GetUsersByNome(nome);
                return Ok(users ?? new List<UserDTOResponse>());
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Requisição inválida ao buscar usuários por nome '{Nome}'", nome);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuários por nome '{Nome}'", nome);
                return Problem(detail: "Erro interno ao processar a requisi��o.", statusCode: 500);
            }
        }

        /// <summary>
        /// Desativa o acesso de um usuário no sistema.
        /// </summary>
        [HttpPut("desativar")]
        public async Task<IActionResult> DesativarUser([FromQuery] string cpf, [FromQuery] string tipo, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(cpf) || string.IsNullOrWhiteSpace(tipo))
                return BadRequest(new { message = "CPF e tipo são obrigatórios." });

            try
            {
                await _userService.DesativarUser(cpf, tipo);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Tentativa de desativar usuário não encontrado: {Cpf} / {Tipo}", MaskCpf(cpf), tipo);
                return NotFound(new { message = "Usuário não encontrado." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Requisição inválida ao desativar usuário {Cpf} / {Tipo}", MaskCpf(cpf), tipo);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar usuário {Cpf} / {Tipo}", MaskCpf(cpf), tipo);
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        /// <summary>
        /// Reativa o acesso de um usuário previamente desativado.
        /// </summary>
        [HttpPut("ativar")]
        public async Task<IActionResult> AtivarUser([FromQuery] string cpf, [FromQuery] string tipo, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(cpf) || string.IsNullOrWhiteSpace(tipo))
                return BadRequest(new { message = "CPF e tipo são obrigatórios." });

            try
            {
                await _userService.AtivarUser(cpf, tipo);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Tentativa de ativar usuário não encontrado: {Cpf} / {Tipo}", MaskCpf(cpf), tipo);
                return NotFound(new { message = "Usuário não encontrado." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Requisição inválida ao ativar usuário {Cpf} / {Tipo}", MaskCpf(cpf), tipo);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao ativar usuário {Cpf} / {Tipo}", MaskCpf(cpf), tipo);
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        // simples mascaramento de CPF para logs (não registrar CPF completo)
        private static string MaskCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return "N/A";

            var clean = cpf.Replace(".", "").Replace("-", "").Trim();
            if (clean.Length < 4)
                return "***";

            var suffix = clean[^2..];
            return $"***.***.***-{suffix}";
        }
    }
}