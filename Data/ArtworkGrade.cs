using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppBackEnd.Data
{
    public class ArtworkGrade
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Artwork))]
        public int ArtworkId { get; set; }
        public Artwork Artwork { get; set; }

        [Required]
        [Range(1, 5)]
        public int Points { get; set; }

        [ForeignKey(nameof(JuryMember))]
        public int JuryMemberId { get;set; }
        public JuryMember JuryMember { get; set; }
    }
}
