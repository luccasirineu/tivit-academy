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

            // Cria entidade ComprovantePagamento
            ComprovantePagamento comprovantePagamento = new ComprovantePagamento
            (
                 matriculaId,
                 arquivoBytes,
                 DateTime.Now
            );

            //  Salva no banco
            _context.ComprovantesPagamento.Add(comprovantePagamento);

            //  Atualiza o status da matrícula
            matricula.Status = "AGUARDANDO_DOCUMENTOS";

            await _context.SaveChangesAsync();

            //  Retorna DTO
            return new ComprovantePagamentoDTO
            (
                 comprovantePagamento.MatriculaId,
                 comprovantePagamento.Arquivo,
                 comprovantePagamento.HoraEnvio
            );
        }



    }
}
