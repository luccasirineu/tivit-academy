using tivitApi.Data;
using tivitApi.Models;
using tivitApi.DTOs;
using tivitApi.Infra.SQS;
using tivitApi.Exceptions;
using tivitApi.Mappers;
using tivitApi.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;



namespace tivitApi.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MatriculaService> _logger;
        private readonly ISQSProducer  _queue;
        private readonly IPasswordHasher _passwordHasher;


        public MatriculaService(AppDbContext context, ILogger<MatriculaService> logger, ISQSProducer  queue, IPasswordHasher passwordHasher)
        {
            _context = context;
            _logger = logger;
            _queue = queue;
            _passwordHasher = passwordHasher;
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
                throw new NotFoundException("Matricula", id);

            return matricula;
        }

        private async Task<Aluno> CriarAlunoAPartirDaMatricula(Matricula matricula, string senha)
        {
            var aluno = new Aluno
            {
                Nome = matricula.Nome,
                Email = matricula.Email,
                Cpf = matricula.Cpf,
                Senha = senha,
                MatriculaId = matricula.Id,
                Status = StatusUsuario.ATIVO
            };

            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            return aluno;
        }


        public async Task<Matricula> CriarMatriculaAsync(MatriculaDTO matriculaDTO)
        {
            _logger.LogInformation("Iniciando criação de matrícula: {@MatriculaDTO}", matriculaDTO);

            var matricula = matriculaDTO.ToEntity();
            matricula.Cpf = SomenteNumeros(matricula.Cpf);

            bool existeNoBanco = await _context.Matriculas.AnyAsync(m =>
                m.Cpf == matricula.Cpf &&
                m.CursoId == matricula.CursoId &&
                (m.Status == StatusMatricula.AGUARDANDO_APROVACAO ||
                 m.Status == StatusMatricula.APROVADO)
            );

            if (!existeNoBanco)
            {
                _context.Matriculas.Add(matricula);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Matrícula criada com sucesso! ID gerado: {MatriculaId}", matricula.Id);
                return matricula;
            }

            _logger.LogWarning("Tentativa de cadastrar CPF já matriculado. CPF: {Cpf}", matricula.Cpf);
            throw new BusinessException("CPF já está em processo de matricula");
        }

        public async Task<ComprovantePagamentoDTO> EnviarComprovantePagamentoAsync(int matriculaId, IFormFile arquivo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
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

                matricula.Status = StatusMatricula.AGUARDANDO_DOCUMENTOS;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new ComprovantePagamentoDTO
                (
                    comprovante.MatriculaId,
                    comprovante.Arquivo,
                    comprovante.HoraEnvio
                );
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<DocumentosDTO> EnviarDocumentosAsync(int matriculaId, IFormFile documentoHistorico, IFormFile documentoCpf)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
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

                matricula.Status = StatusMatricula.AGUARDANDO_APROVACAO;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new DocumentosDTO
                (
                    documentos.MatriculaId,
                    documentos.DocumentoHistorico,
                    documentos.DocumentoCpf,
                    documentos.HoraEnvio
                );
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<MatriculaDTO>> GetAllMatriculasPendentes()
        {
            _logger.LogInformation("Pegando todos as matriculas pendentes");

            var matriculas = await _context.Matriculas
                .Where(c => c.Status == StatusMatricula.AGUARDANDO_APROVACAO)
                .ToListAsync();

            return matriculas.Select(matricula => matricula.ToDTO()).ToList();
        }


        public async Task AprovarMatricula(string matriculaId)
        {
            _logger.LogInformation("Aprovando matricula: {MatriculaId}", matriculaId);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int matriculaIdConvertida = int.Parse(matriculaId);
                var matricula = await ObterMatricula(matriculaIdConvertida);

                matricula.Status = StatusMatricula.APROVADO;
                await _context.SaveChangesAsync();

                var senhaGerada = GerarSenha();
                var senhaHash = _passwordHasher.Hash(senhaGerada);

                await CriarAlunoAPartirDaMatricula(matricula, senhaHash);

                await transaction.CommitAsync();

                // Envio de evento SQS fora da transação (operação externa)
                try
                {
                    await _queue.EnviarEventoAsync(new MatriculaStatusEvento
                    {
                        MatriculaId = matricula.Id,
                        Nome = matricula.Nome,
                        Email = matricula.Email,
                        Status = StatusMatricula.APROVADO.ToString(),
                        SenhaGerada = senhaGerada,
                        Cpf = matricula.Cpf
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao enviar evento para SQS");
                    // Não falha a operação se o envio do evento falhar
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao aprovar matricula");
                throw;
            }
        }

        public async Task RecusarMatricula(string matriculaId)
        {
            _logger.LogInformation("Reprovando matricula: {MatriculaId}", matriculaId);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int matriculaIdConvertida = int.Parse(matriculaId);
                var matricula = await ObterMatricula(matriculaIdConvertida);

                matricula.Status = StatusMatricula.RECUSADO;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Envio de evento SQS fora da transação (operação externa)
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
                    _logger.LogError(ex, "Erro ao enviar evento para SQS");
                    // Não falha a operação se o envio do evento falhar
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao reprovar matricula");
                throw;
            }
        }

        public async Task<int> GetTotalAlunosAtivosPorProfessor(int professorId)
        {
            var cursosIds = await _context.Cursos
                .Where(c => c.ProfResponsavel == professorId)
                .Select(c => c.Id)
                .ToListAsync();

            if (!cursosIds.Any())
                return 0;

            return await _context.Matriculas
                .Where(m => cursosIds.Contains(m.CursoId) && m.Status == StatusMatricula.APROVADO)
                .CountAsync();
        }
    }
}

