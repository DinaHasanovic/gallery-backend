namespace AppBackEnd.DTO
{
    public class UpdateToJournalistRequestDTO
    {
        public string agency { get; set; }
        public int exhibitionId { get; set; }
        public DateTime date { get; set; }
    }
}
