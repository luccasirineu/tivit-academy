using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tivitApi.Models
{
    public class Materia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string Descricao { get; set; }

        // FK 
        [ForeignKey("Curso")]
        public int CursoId { get; set; }
        public Curso curso { get; set; }

        public ICollection<Conteudo> Conteudos { get; set; }


        public Materia(string nome, string descricao, int cursoId)
        {
            Nome = nome;
            Descricao = descricao;
            CursoId = cursoId;
        }

        public Materia() { }

    }
}
