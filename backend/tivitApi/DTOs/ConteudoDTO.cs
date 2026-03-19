namespace tivitApi.DTOs
{

    public class ConteudoDTO
    {

        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Tipo { get; set; }
        public string CaminhoOuUrl { get; set; }
        public DateTime DataPublicacao { get; set; }
        public int MateriaId { get; set; }
        public int ProfessorId { get; set; }
        public int TurmaId { get; set; }



        public ConteudoDTO(int id, string titulo, string tipo, string caminhoOuUrl, DateTime DataPublicao, int materiaId, int professorId, int turmaId)
        {
            Id = id;
            Titulo = titulo;
            Tipo = tipo;
            CaminhoOuUrl = caminhoOuUrl;
            DataPublicacao = DataPublicao;
            MateriaId = materiaId;
            ProfessorId = professorId;
            TurmaId = turmaId;
        }

        public ConteudoDTO() { }

    }
}
