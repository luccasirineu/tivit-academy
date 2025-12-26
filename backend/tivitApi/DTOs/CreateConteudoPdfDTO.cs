namespace tivitApi.DTOs
{
    public class CreateConteudoPdfDTO
    {
        public string Titulo { get; set; }
        public int MateriaId { get; set; }
        public IFormFile Arquivo { get; set; }
    }
}