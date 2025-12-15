using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tivitApi.Models
{
    public class Evento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Descricao { get; set; }

        [Required]
        public DateTime Horario { get; set; }



        public Evento(string titulo, string descricao, DateTime horario)
        {
            Titulo = titulo;
            Descricao = descricao;
            Horario = horario;
        }


        public Evento() { }

    }
}
