namespace tivitApi.DTOs 
{

    public class MatriculaStatusEvento
    {
        public int MatriculaId { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string SenhaGerada { get; set; }
        public string Cpf { get; set; }
    }
}
