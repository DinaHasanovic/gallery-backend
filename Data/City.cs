using System.ComponentModel.DataAnnotations;

namespace AppBackEnd.Data
{
    public class City
    {

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]   
        public string PTT { get; set; }

    }
}
