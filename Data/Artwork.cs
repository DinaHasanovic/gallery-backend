using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppBackEnd.Data
{
    public class Artwork
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Dimensions { get; set; }
        [Required]
        public string UniqueCode { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [ForeignKey(nameof(Painter))]
        public int PainterId { get; set; }
        public Painter Painter { get; set; }

        [ForeignKey(nameof(Hall))]
        public int? HallId { get; set; }
        public Hall? Hall { get; set; }

        List<ArtworkGrade> Grades { get; set; }

        [ForeignKey(nameof(Theme))]
        public int? ThemeId { get; set; }
        public Theme? Theme { get; set; }
    }
}
