
namespace tivitApi.DTOs
{
    public class LoginDTOResponse
    {
        public int Id { get; set; }        
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string Cpf { get; set; }
        public List<int> CursosIds { get; set; }
        public int TurmaId { get; set; }
        public string Token { get; set; }
    }
}
