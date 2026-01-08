using Microsoft.EntityFrameworkCore;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Exceptions;

namespace tivitApi.Services
{

    public interface INotaService
    {
        Task<NotaDTOResponse> AdicionarNotaAsync(NotaDTORequest notaDTO);
        Task<List<NotaDTOResponse>> GetAllNotasByAlunoId(int alunoId);
        Task<DesempenhoDTO> GetDesempenhoByAlunoId(int alunoId);

    }

    public class NotaService : INotaService
    {
        private readonly AppDbContext _context;

        public NotaService(AppDbContext context)
        {
            _context = context;
        }

        private Nota ConvertNotaDtoToNota(NotaDTORequest notaDTO, decimal media, string status)
        {
            return new Nota(
                notaDTO.AlunoId,
                notaDTO.MateriaId,
                notaDTO.Nota1,
                notaDTO.Nota2,
                media,
                notaDTO.QtdFaltas,
                status
                );
        }


        private decimal CalcularMedia(decimal nota1, decimal nota2)
        {

            var media = (nota1 + nota2) / 2;

            return media;
        }

        private string CalcularStatusNota(decimal media, int qntdFaltas)
        {
            if (qntdFaltas > 20) return "REPROVADO";

            if (media >= 6) return "APROVADO";
            return "REPROVADO";
        }

        public async Task<NotaDTOResponse> AdicionarNotaAsync(NotaDTORequest dto)
        {
            var alunoExiste = await _context.Alunos
                .AnyAsync(a => a.Id == dto.AlunoId);

            if (!alunoExiste)
                throw new Exception("Aluno não encontrado.");

            var materiaExiste = await _context.Materias
                .AnyAsync(m => m.Id == dto.MateriaId);

            if (!materiaExiste)
                throw new Exception("Matéria não encontrada.");

            
            if (dto.Nota1 < 0 || dto.Nota1 > 10 ||
                dto.Nota2 < 0 || dto.Nota2 > 10)
                throw new Exception("Notas devem estar entre 0 e 10.");

            if (dto.QtdFaltas < 0)
                throw new Exception("Quantidade de faltas inválida.");

            decimal media = CalcularMedia(dto.Nota1, dto.Nota2);
            string status = CalcularStatusNota(media, dto.QtdFaltas);
            var nota = ConvertNotaDtoToNota(dto,media,status);

            _context.Notas.Add(nota);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Erro ao salvar a nota. Verifique se já existe cadastro para este aluno e matéria.");
            }

            return new NotaDTOResponse
            {
                AlunoId = nota.AlunoId,
                MateriaId = nota.MateriaId,
                Nota1 = nota.Nota1,
                Nota2 = nota.Nota2,
                Media = media,
                QtdFaltas = nota.QtdFaltas,
                Status = status
            };
        }

        public async Task<List<NotaDTOResponse>> GetAllNotasByAlunoId(int alunoId)
        {
            // Valida se o aluno existe
            var alunoExiste = await _context.Alunos.AnyAsync(a => a.Id == alunoId);
            if (!alunoExiste)
                throw new Exception("Aluno não encontrado.");

            var notas = await _context.Notas
                .Where(n => n.AlunoId == alunoId)
                .Select(n => new NotaDTOResponse
                {
                    AlunoId = n.AlunoId,
                    MateriaId = n.MateriaId,
                    Nota1 = n.Nota1,
                    Nota2 = n.Nota2,
                    Media = n.Media,
                    QtdFaltas = n.QtdFaltas,
                    Status = n.Status
                })
                .ToListAsync();

            return notas;
        }

        public async Task<DesempenhoDTO> GetDesempenhoByAlunoId(int alunoId)
        {
            var notasAluno = await _context.Notas
                .Where(n => n.AlunoId == alunoId)
                .OrderByDescending(n => n.Media)
                .FirstOrDefaultAsync();

            if (notasAluno == null)
                throw new Exception("Nenhuma nota encontrada para este aluno.");

            var materia = await _context.Materias
                .Where(m => m.Id == notasAluno.MateriaId)
                .Select(m => m.Nome)
                .FirstOrDefaultAsync();

            if (materia == null)
                throw new Exception("Matéria não encontrada.");

            string nivel;

            if (notasAluno.Media < 6)
                nivel = "BRONZE";
            else if (notasAluno.Media <= 8)
                nivel = "PRATA";
            else
                nivel = "OURO";

            return new DesempenhoDTO
            {
                NomeMateria = materia,
                Media = notasAluno.Media,
                QtdFaltas = notasAluno.QtdFaltas,
                Nivel = nivel
            };
        }



    }
}