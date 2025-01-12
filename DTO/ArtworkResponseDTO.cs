namespace AppBackEnd.DTO
{
    public class ArtworkResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string UniqueCode { get; set; }
        public string Dimensions { get; set; }
        public string ImageUrl {  get; set; }
        public ArtworkPainterDTO Painter{ get; set; }
        public HallResponseDTO   Hall{ get; set; }
        public ThemeDataDTO Theme { get; set; }
    }
}
