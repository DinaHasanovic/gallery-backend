namespace AppBackEnd.DTO
{
    public class LoginUserResponseDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Role { get; set; }
        public string Token { get; set; }
    }
}
