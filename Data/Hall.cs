using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace AppBackEnd.Data
{
    public class Hall
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        [Required]
        public float Surface {  get; set; }

        [ForeignKey(nameof(City))]
        public int? CityId { get; set; } 
        public City? City { get; set; }

        List<Artwork> Artworks { get; set; }
    }
}
