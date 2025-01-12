namespace AppBackEnd.DTO
{
    public class CreateArtworkRequestDTO
    {
        public string Title { get; set; }
        public string Dimensions { get; set; }
        public string UniqueCode { get; set; }
        public int PainterId { get; set; }
        public int ThemeId { get; set; }
        public int HallId { get; set; }
        public IFormFile Image { get; set; }
    }
}
