using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tivitApi.Models
{

    public class NotificacaoTurma
    {
        public int NotificacaoId { get; set; }
        public Notificacao Notificacao { get; set; }

        public int TurmaId { get; set; }
        public Turma Turma { get; set; }
    }
}