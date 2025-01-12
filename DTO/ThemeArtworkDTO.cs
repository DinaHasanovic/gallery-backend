namespace AppBackEnd.DTO
{
    public class ThemeArtworkDTO
    {
        public int Id { get; set; }
        public int PainterId { get; set; }
        public int HallId { get; set; }
        public string Title { get; set; }
        public string Dimensions { get; set; }
        public string UniqueCode { get; set; }
    }
}
