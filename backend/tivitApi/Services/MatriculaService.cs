using tivitApi.Data;
using tivitApi.Models;
using tivitApi.DTOs;
using tivitApi.Infra.SQS;
using tivitApi.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;



namespace tivitApi.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MatriculaService> _logger;
        private readonly SQSProducer _queue;

        public MatriculaService(AppDbContext context, ILogger<MatriculaService> logger, SQSProducer queue)
        {
            _context = context;
            _logger = logger;
            _queue = queue;

        }




        private Matricula ConvertMatriculaDtoToMatricula(MatriculaDTO matriculaDTO)
        {
            return new Matricula(
                matriculaDTO.Nome,
                matriculaDTO.Email,
                matriculaDTO.Cpf,
                matriculaDTO.CursoId);


        }

        private MatriculaDTO ConvertMatriculaToMatriculaDTO(Matricula matricula)
        {
            return new MatriculaDTO(
                matricula.Id,
                matricula.Nome,
                matricula.Email,
                matricula.Cpf,
                matricula.Status,
                matricula.CursoId
                );
        }

        private string SomenteNumeros(string valor)
        {
            return new string(valor.Where(char.IsDigit).ToArray());
        }

        private string GerarSenha(int tamanho = 12)
        {
            const string caracteres =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%&*";

            // Gera bytes criptograficamente seguros
            byte[] bytes = RandomNumberGenerator.GetBytes(tamanho);

            var sb = new StringBuilder();

            foreach (byte b in bytes)
            {
                sb.Append(caracteres[b % caracteres.Length]);
            }

            return sb.ToString();
        }


        public async Task<Matricula> CriarMatriculaAsync(MatriculaDTO matriculaDTO)
        {
            _logger.LogInformation($"Iniciando criação de matrícula: {JsonSerializer.Serialize(matriculaDTO)}");

            var matricula = ConvertMatriculaDtoToMatricula(matriculaDTO);
            matricula.Cpf = SomenteNumeros(matricula.Cpf);

            bool existeNoBanco = await _context.Matriculas.AnyAsync(m =>
                m.Cpf == matricula.Cpf &&                 // 1) Mesmo CPF
                m.CursoId == matricula.CursoId &&         // 2) Mesmo curso
                (m.Status == "AGUARDANDO_APROVACAO" ||    // 3) Status válido
                 m.Status == "APROVADO")
            );

            if (!existeNoBanco)
            {
                _context.Matriculas.Add(matricula);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Matrícula criada com sucesso! ID gerado: {matricula.Id}");
                return matricula;
            }

            _logger.LogWarning("Tentativa de cadastrar CPF já matriculado. CPF: {Cpf}", matricula.Cpf);
            throw new BusinessException("CPF já está em processo de matricula");
        }


        private async Task<byte[]> LerArquivoAsync(IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }

        private async Task<Matricula> ObterMatricula(int id)
        {
            var matricula = await _context.Matriculas.FindAsync(id);
            if (matricula == null)
                throw new Exception("Matrícula não encontrada.");

            return matricula;
        }



        public async Task<ComprovantePagamentoDTO> EnviarComprovantePagamentoAsync(int matriculaId, IFormFile arquivo)
        {
            var matricula = await ObterMatricula(matriculaId);

            var arquivoBytes = await LerArquivoAsync(arquivo);

            var comprovante = new ComprovantePagamento
            (
                matriculaId,
                arquivoBytes,
                DateTime.Now
            );

            _context.ComprovantesPagamento.Add(comprovante);

            matricula.Status = "AGUARDANDO_DOCUMENTOS";
            await _context.SaveChangesAsync();

            return new ComprovantePagamentoDTO
            (
                comprovante.MatriculaId,
                comprovante.Arquivo,
                comprovante.HoraEnvio
            );
        }



        public async Task<DocumentosDTO> EnviarDocumentosAsync(int matriculaId, IFormFile documentoHistorico, IFormFile documentoCpf)
        {
            var matricula = await ObterMatricula(matriculaId);

            var historicoBytes = await LerArquivoAsync(documentoHistorico);
            var cpfBytes = await LerArquivoAsync(documentoCpf);

            var documentos = new Documentos
            (
                matriculaId,
                historicoBytes,
                cpfBytes,
                DateTime.Now
            );

            _context.Documentos.Add(documentos);

            matricula.Status = "AGUARDANDO_APROVACAO";
            await _context.SaveChangesAsync();

            return new DocumentosDTO
            (
                documentos.MatriculaId,
                documentos.DocumentoHistorico,
                documentos.DocumentoCpf,
                documentos.HoraEnvio
            );
        }

        public async Task<List<MatriculaDTO>> GetAllMatriculasPendentes()
        {
            _logger.LogInformation("Pegando todos as matriculas pendentes");

            var matriculas = await _context.Matriculas.Where(c => c.Status == "AGUARDANDO_APROVACAO").ToListAsync();

            List<MatriculaDTO> matriculasDTO = new List<MatriculaDTO>();

            foreach (var matricula in matriculas)
            {
                matriculasDTO.Add(ConvertMatriculaToMatriculaDTO(matricula));
            }

            return matriculasDTO;
        }


        public async Task AprovarMatricula(string matriculaId)
        {
            _logger.LogInformation($"Aprovando matricula: {matriculaId}");

            try
            {
                int matriculaIdConvertida = int.Parse(matriculaId);
                var matricula = await ObterMatricula(matriculaIdConvertida);

                matricula.Status = "APROVADO";
                await _context.SaveChangesAsync();

                var senhaGerada = GerarSenha();

                try
                {
                    await _queue.EnviarEventoAsync(new MatriculaStatusEvento
                    {
                        MatriculaId = matricula.Id,
                        Nome = matricula.Nome,
                        Email = matricula.Email,
                        Status = "APROVADO",
                        SenhaGerada = senhaGerada
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao enviar evento para SQS: {ex.Message}");
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao aprovar matricula: {ex.Message}");
                throw;
            }
        }


        public async Task RecusarMatricula(string matriculaId)
        {
            _logger.LogInformation($"Reprovando matricula: {matriculaId}");


            try
            {
                int matriculaIdConvertida = int.Parse(matriculaId);
                var matricula = await ObterMatricula(matriculaIdConvertida);

                matricula.Status = "RECUSADO";
                await _context.SaveChangesAsync();


                try
                {
                    await _queue.EnviarEventoAsync(new MatriculaStatusEvento
                    {
                        MatriculaId = matricula.Id,
                        Nome = matricula.Nome,
                        Email = matricula.Email,
                        Status = "RECUSADO"
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao enviar evento para SQS: {ex.Message}");
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao aprovar matricula: {ex.Message}");
                throw;
            }
        }

    }
}

