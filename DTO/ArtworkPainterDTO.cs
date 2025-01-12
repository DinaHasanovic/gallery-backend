using AppBackEnd.Data;

namespace AppBackEnd.DTO
{
    public class ArtworkPainterDTO
    {
        public int Id { get; set; }
        public User User { get; set; }
        public string JMBG { get; set; }
    }
}
