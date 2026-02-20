using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tivitApi.Models
{
    public class Turma
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        // FK 
        [ForeignKey("Curso")]
        public int CursoId { get; set; }
        public Curso Curso { get; set; }

        public string Status { get; set; }

        public ICollection<NotificacaoTurma> NotificacaoTurmas { get; set; }


        public Turma( string nome, int cursoId, string status)
        {
            Nome = nome;
            CursoId = cursoId;
            Status = status;
        }

    }
}
