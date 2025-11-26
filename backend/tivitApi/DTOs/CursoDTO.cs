namespace tivitApi.DTOs
{
    public class CursoDTO
    {
        public int Id { get; set; }          

        public string Nome { get; set; }

        public string Descricao { get; set; }  

        public int ProfResponsavel { get; set; }

        public CursoDTO(int id, string nome, string descricao, int profResponsavel)
        {
            Id = id;
            Nome = nome;
            Descricao = descricao;
            ProfResponsavel = profResponsavel;
        }

        public CursoDTO() { }


    }
}
