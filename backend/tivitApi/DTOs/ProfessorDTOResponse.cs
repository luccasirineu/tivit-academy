namespace tivitApi.DTOs
{
    public class ProfessorDTOResponse
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }

        public string Rm { get; set; }

        public string Cpf { get; set; }

        public string Status { get; set; }  

        public ProfessorDTOResponse(int id, string nome, string email, string rm, string cpf, string status)
        {
            Id = id;
            Nome = nome;
            Email = email;
            Rm = rm;
            Cpf = cpf;
            Status = status;
        }

        public ProfessorDTOResponse() { }


    }
}
