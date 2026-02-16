namespace tivitApi.DTOs
{
    public class ProfessorDTORequest
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }



        public string Cpf { get; set; }

        public string Status { get; set; }


        public ProfessorDTORequest(int id, string nome, string email, string cpf, string status)
        {
            Id = id;
            Nome = nome;
            Email = email;
            Cpf = cpf;
            Status = status;
        }

        public ProfessorDTORequest() { }


    }
}
