using System.ComponentModel.DataAnnotations;

namespace AppBackEnd.DTO
{
    public class RegisterUserRequestDTO
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName  { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
