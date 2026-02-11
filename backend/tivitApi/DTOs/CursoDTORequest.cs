namespace tivitApi.DTOs
{
    public class CursoDTORequest
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Descricao { get; set; }

        public int ProfResponsavel { get; set; }


        public CursoDTORequest(string nome, string descricao, int profResponsavel, string status)
        {
            Nome = nome;
            Descricao = descricao;
            ProfResponsavel = profResponsavel;
        }

        public CursoDTORequest(int id, string nome, string descricao, int profResponsavel, string status)
        {
            Id = id;
            Nome = nome;
            Descricao = descricao;
            ProfResponsavel = profResponsavel;
        }

        public CursoDTORequest() { }


    }
}
