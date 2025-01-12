namespace AppBackEnd.DTO
{
    public class CreateExhibitionRequestDTO
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int HallId { get; set; }
    }
}
