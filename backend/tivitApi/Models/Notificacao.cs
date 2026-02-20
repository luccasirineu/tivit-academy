using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tivitApi.Models
{
    public class Notificacao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Descricao { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;


        // Navegação para a tabela intermediária
        public ICollection<NotificacaoTurma> NotificacaoTurmas { get; set; }

        public Notificacao(string titulo, string descricao)
        {
            Titulo = titulo;
            Descricao = descricao;
        }

    }

    

}
