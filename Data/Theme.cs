using System.ComponentModel.DataAnnotations;

namespace AppBackEnd.Data
{
    public class Theme
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<Artwork> Artworks { get; set; }
    }
}
