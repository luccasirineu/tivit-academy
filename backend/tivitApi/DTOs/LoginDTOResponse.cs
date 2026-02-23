
namespace tivitApi.DTOs
{
    public class LoginDTOResponse
    {
        public string Tipo { get; set; }
        public string Cpf { get; set; }
        public List<int> CursosIds { get; set; }
    }
}
