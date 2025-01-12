using System.Reflection.Metadata.Ecma335;

namespace AppBackEnd.DTO
{
    public class ExhibitionResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int HallId { get; set; }
        public List<ExhibitionArtworkDTO> Artworks { get; set; }
    }
}
