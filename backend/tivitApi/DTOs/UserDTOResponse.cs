namespace tivitApi.DTOs
{

    public class UserDTOResponse
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }
        public string Tipo { get; set; }
        public string Status { get; set; }

        public UserDTOResponse(string nome, string email, string cpf, string tipo, string status)
        {
            Nome = nome;
            Email = email;
            Cpf = cpf;
            Tipo = tipo;
            Status = status;
        }


        public UserDTOResponse() { }

    }
}
