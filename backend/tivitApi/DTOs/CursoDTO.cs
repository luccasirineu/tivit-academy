namespace tivitApi.DTOs
{
    public class CursoDTO
    {
        public string Nome { get; set; }

        public int ProfResponsavel { get; set; }

        public CursoDTO(string nome, int profResponsavel)
        {
            Nome = nome;
            ProfResponsavel = profResponsavel;
        }

        public CursoDTO() { }


    }
}
