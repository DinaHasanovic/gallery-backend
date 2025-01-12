namespace AppBackEnd.DTO
{
    public class CreateGradeRequestDTO
    {
        public int JuryMemberId { get; set; }
        public int ArtworkId { get; set; }
        public int Points { get; set; }
    }
}
