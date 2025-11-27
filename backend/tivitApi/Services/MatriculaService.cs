using tivitApi.Data;
using tivitApi.Models;
using tivitApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace tivitApi.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly AppDbContext _context;

        public MatriculaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Matricula> CriarMatriculaAsync(MatriculaDTO matriculaDTO)
        {
            var matricula = ConvertMatriculaDtoToMatricula(matriculaDTO);
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();
            return matricula;
        }

        private Matricula ConvertMatriculaDtoToMatricula(MatriculaDTO matriculaDTO)
        {
            return new Matricula(
                matriculaDTO.Nome,
                matriculaDTO.Email,
                matriculaDTO.Cpf,
                matriculaDTO.CursoId);
               

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
