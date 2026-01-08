using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tivitApi.Models
{
    public class Nota
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AlunoId { get; set; }

        [Required]
        public int MateriaId { get; set; }

        public decimal Nota1 { get; set; }

        public decimal Nota2 { get; set; }

        public decimal Media { get; set; }

        public int QtdFaltas { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } 

        public Aluno Aluno { get; set; }
        public Materia Materia { get; set; }


        public Nota(int alunoId, int materiaId, decimal nota1, decimal nota2, decimal media, int qtdFaltas, string status )
        {
            AlunoId = alunoId;
            MateriaId = materiaId;
            Nota1 = nota1;
            Nota2 = nota2;
            Media = media;
            QtdFaltas = qtdFaltas;
            Status = status;
        }
    }

}
