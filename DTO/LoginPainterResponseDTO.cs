using AppBackEnd.Data;

namespace AppBackEnd.DTO
{
    public class LoginPainterResponseDTO
    {
        public int Id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string JMBG { get; set; }
        public string cityName { get; set; }
        public int Role { get; set; }
        public string Token { get; set; }
    }
}
