using AppBackEnd.Data;

namespace AppBackEnd.DTO
{
    public class LoginJournalistResponseDTO
    {
        public int Id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string agency { get; set; }
        public int Role { get; set; }
        public string Token { get; set; }
    }
}
