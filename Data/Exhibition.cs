using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppBackEnd.Data
{
    public class Exhibition
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [ForeignKey(nameof(Hall))]
        public int? HallId { get; set; }
        public Hall? Hall { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public List<JournalistVisit> Visits { get; set; }

        public List<Artwork> Artworks { get; set; }
    }
}
