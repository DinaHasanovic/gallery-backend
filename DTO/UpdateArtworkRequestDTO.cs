namespace AppBackEnd.DTO
{
    public class UpdateArtworkRequestDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? UniqueCode { get; set; }
        public int? HallId { get; set; }
        public int? ThemeId { get; set; }
        public string? Dimensions { get; set; }
    }
}
