using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppBackEnd.Data
{
    public class JuryMember
    {
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [StringLength(13, MinimumLength = 13)]
        [RegularExpression(@"^\d{13}$")]
        public string JMBG { get; set; }

        List<ArtworkGrade> Grades { get; set; }

    }
}
