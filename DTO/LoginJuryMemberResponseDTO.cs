using AppBackEnd.Data;

namespace AppBackEnd.DTO
{
    public class LoginJuryMemberResponseDTO
    {
        public int Id { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int role { get; set; }
        public string Token { get; set; }
    }
}
