namespace AppBackEnd.DTO
{
    public class VisitResponseDTO
    {
        public int Id { get; set; }
        public VisitJournalistDTO Journalist { get; set; }
        public ExhibitionResponseDTO Exhibition { get; set; }
        public DateTime Date { get; set; }
    }
}
