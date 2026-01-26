using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tivitApi.Models
{
    public class Conteudo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Tipo { get; set; }

        [Required]
        public string CaminhoOuUrl { get; set; }

        [Required]
        public DateTime DataPublicacao { get; set; }

        [ForeignKey("Materia")]
        public int MateriaId { get; set; }
        public Materia Materia { get; set; }

        [ForeignKey("Professor")]
        public int ProfessorId { get; set; }
        public Professor Professor { get; set; }

        [Required]
        public int TurmaId { get; set; }

    }
}

