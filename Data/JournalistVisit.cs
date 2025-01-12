using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppBackEnd.Data
{
    public class JournalistVisit
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Journalist))]
        public int JournalistId { get; set; }
        public Journalist Journalist { get; set; }

        [ForeignKey(nameof(Exhibition))]
        public int ExhibitionId { get; set; }
        public Exhibition Exhibition { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
