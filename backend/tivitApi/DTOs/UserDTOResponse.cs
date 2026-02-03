namespace tivitApi.DTOs
{

    public class UserDTOResponse
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }


        public UserDTOResponse(string nome, string email, string cpf)
        {
            Nome = nome;
            Email = email;
            Cpf = cpf;
        }


        public UserDTOResponse() { }

    }
}
