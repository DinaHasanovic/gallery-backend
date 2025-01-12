using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppBackEnd.Data
{
    public class Journalist
    {
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public string Agency { get; set; }

        [MinLength(1)]
        List<JournalistVisit> JournalistVisits { get; set; }
    }
}
