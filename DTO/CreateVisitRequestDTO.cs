namespace AppBackEnd.DTO
{
    public class CreateVisitRequestDTO
    {
        public int JournalistId { get; set; }
        public int ExhibitionId { get; set; }
        public DateTime Date { get; set; }
    }
}
