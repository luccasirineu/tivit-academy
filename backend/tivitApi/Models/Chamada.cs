using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tivitApi.Models
{
    public class Chamada
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MatriculaId { get; set; }

        [Required]
        public int MateriaId { get; set; }

        [Required]
        public bool Faltou { get; set; }

        [Required]
        public DateTime HorarioDaAula{ get; set; }


        public Matricula Matricula { get; set; }
        public Materia Materia { get; set; }


        public Chamada(int matriculaId, int materiaId, bool faltou, DateTime horarioDaAula)
        {
            MatriculaId = matriculaId;
            MateriaId = materiaId;
            Faltou = faltou;
            HorarioDaAula = horarioDaAula;
        }
    }

}
