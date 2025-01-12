using System.ComponentModel.DataAnnotations.Schema;

namespace AppBackEnd.Data
{
    public class ExhibitionArtwork
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Artwork))]
        public int ArtworkId { get; set; }
        public Artwork Artwork { get; set; }

        [ForeignKey(nameof(Exhibition))]
        public int ExhibitionId { get;set; }
        public Exhibition Exhibition { get; set; }

    }
}
