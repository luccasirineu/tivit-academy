using tivitApi.Data;
using tivitApi.Models;
using tivitApi.DTOs;
using tivitApi.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace tivitApi.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MatriculaService> _logger;

        public MatriculaService(AppDbContext context, ILogger<MatriculaService> logger)
        {
            _context = context;
            _logger = logger;
        }




        private Matricula ConvertMatriculaDtoToMatricula(MatriculaDTO matriculaDTO)
        {
            return new Matricula(
                matriculaDTO.Nome,
                matriculaDTO.Email,
                matriculaDTO.Cpf,
                matriculaDTO.CursoId);


        }

        private string SomenteNumeros(string valor)
        {
            return new string(valor.Where(char.IsDigit).ToArray());
        }


        public async Task<Matricula> CriarMatriculaAsync(MatriculaDTO matriculaDTO)
        {
            _logger.LogInformation($"Iniciando criação de matrícula: {JsonSerializer.Serialize(matriculaDTO)}");

            var matricula = ConvertMatriculaDtoToMatricula(matriculaDTO);
            matricula.Cpf = SomenteNumeros(matricula.Cpf);

            bool existeNoBanco = await _context.Matriculas.AnyAsync(m =>
                  (m.Cpf == matricula.Cpf || m.Email == matriculaDTO.Email) &&
                  (m.Status == "AGUARDANDO_APROVACAO" || m.Status == "APROVADO")
              );

            if (!existeNoBanco) {
                _context.Matriculas.Add(matricula);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Matrícula criada com sucesso! ID gerado: {matricula.Id}");
                return matricula;
            }

            _logger.LogWarning("Tentativa de cadastrar CPF ou Email já existente. CPF: {Cpf}, Email: {Email}", matricula.Cpf, matriculaDTO.Email);
            throw new BusinessException("Email ou CPF já cadastrado");
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




    }
}
