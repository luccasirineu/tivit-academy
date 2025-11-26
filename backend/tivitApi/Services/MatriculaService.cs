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

        public async Task<Matricula> CriarMatriculaAsync(Matricula matricula)
        {
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();
            return matricula;
        }

        public Matricula ConvertMatriculaDtoToMatricula(MatriculaDTO matriculaDTO)
        {
            return new Matricula(
                matriculaDTO.Nome,
                matriculaDTO.Email,
                matriculaDTO.Cpf,
                matriculaDTO.CursoId);
               

        }


        public async Task<ComprovantePagamentoDTO> EnviarComprovantePagamentoAsync(int matriculaId, IFormFile arquivo)
        {
            var matricula = await _context.Matriculas.FindAsync(matriculaId);

            if (matricula == null)
                throw new Exception("Matrícula não encontrada.");

            // Converte IFormFile → byte[]
            byte[] arquivoBytes;
            using (var ms = new MemoryStream())
            {
                await arquivo.CopyToAsync(ms);
                arquivoBytes = ms.ToArray();
            }

            ComprovantePagamento comprovantePagamento = new ComprovantePagamento
            (
                 matriculaId,
                 arquivoBytes,
                 DateTime.Now
            );

            _context.ComprovantesPagamento.Add(comprovantePagamento);

            matricula.Status = "AGUARDANDO_DOCUMENTOS";

            await _context.SaveChangesAsync();

            return new ComprovantePagamentoDTO
            (
                 comprovantePagamento.MatriculaId,
                 comprovantePagamento.Arquivo,
                 comprovantePagamento.HoraEnvio
            );
        }

        public async Task<DocumentosDTO> EnviarDocumentosAsync(int matriculaId, IFormFile documentoHistorico, IFormFile documentoCpf)
        {
            var matricula = await _context.Matriculas.FindAsync(matriculaId);

            if (matricula == null)
                throw new Exception("Matrícula não encontrada.");

            // Converte IFormFile → byte[]
            byte[] documentoHistoricoBytes;
            using (var ms = new MemoryStream())
            {
                await documentoHistorico.CopyToAsync(ms);
                documentoHistoricoBytes = ms.ToArray();
            }

            byte[] documentoCpfBytes;
            using (var ms = new MemoryStream())
            {
                await documentoCpf.CopyToAsync(ms);
                documentoCpfBytes = ms.ToArray();
            }

            Documentos documentos = new Documentos
            (
                 matriculaId,
                 documentoHistoricoBytes,
                 documentoCpfBytes,
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
