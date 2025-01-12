namespace AppBackEnd.DTO
{
    public class ThemeResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ThemeArtworkDTO> Artworks { get; set; }
    }
}
