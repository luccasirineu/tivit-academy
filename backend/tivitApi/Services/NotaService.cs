using Microsoft.EntityFrameworkCore;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Exceptions;

namespace tivitApi.Services
{

    public interface INotaService
    {
        Task<NotaDTO> AdicionarNotaAsync(NotaDTO notaDTO);
        Task<List<NotaDTO>> BuscarNotasPorAlunoAsync(int alunoId);
    }

    public class NotaService : INotaService
    {
        private readonly AppDbContext _context;

        public NotaService(AppDbContext context)
        {
            _context = context;
        }

        private Nota ConvertNotaDtoToNota(NotaDTO notaDTO)
        {
            return new Nota(
                notaDTO.AlunoId,
                notaDTO.MateriaId,
                notaDTO.Nota1,
                notaDTO.Nota2,
                notaDTO.QtdFaltas,
                notaDTO.Status
                );
        }

        private string CalcularStatusNota(decimal nota1, decimal nota2, int qntdFaltas)
        {
            if (qntdFaltas > 20) return "REPROVADO";

            var media = (nota1 + nota2) / 2;

            if (media >= 6) return "APROVADO";
            return "REPROVADO";
        }

        public async Task<NotaDTO> AdicionarNotaAsync(NotaDTO dto)
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

            dto.Status = CalcularStatusNota(dto.Nota1, dto.Nota2, dto.QtdFaltas);
            var nota = ConvertNotaDtoToNota(dto);

            _context.Notas.Add(nota);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Erro ao salvar a nota. Verifique se já existe cadastro para este aluno e matéria.");
            }

            return new NotaDTO
            {
                AlunoId = nota.AlunoId,
                MateriaId = nota.MateriaId,
                Nota1 = nota.Nota1,
                Nota2 = nota.Nota2,
                QtdFaltas = nota.QtdFaltas,
                Status = nota.Status
            };
        }



        public async Task<List<NotaDTO>> BuscarNotasPorAlunoAsync(int alunoId)
        {
            // Valida se o aluno existe
            var alunoExiste = await _context.Alunos.AnyAsync(a => a.Id == alunoId);
            if (!alunoExiste)
                throw new Exception("Aluno não encontrado.");

            var notas = await _context.Notas
                .Where(n => n.AlunoId == alunoId)
                .Select(n => new NotaDTO
                {
                    AlunoId = n.AlunoId,
                    MateriaId = n.MateriaId,
                    Nota1 = n.Nota1,
                    Nota2 = n.Nota2,
                    QtdFaltas = n.QtdFaltas,
                    Status = n.Status
                })
                .ToListAsync();

            return notas;
        }




    }
}