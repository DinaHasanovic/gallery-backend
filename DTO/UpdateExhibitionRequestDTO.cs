namespace AppBackEnd.DTO
{
    public class UpdateExhibitionRequestDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int? HallId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
