using AppBackEnd.Data;

namespace AppBackEnd.DTO
{
    public class VisitJournalistDTO
    {
        public int Id { get; set; }
        public string Agency { get; set; }
        public UserSearchResponseDTO User{ get; set; }
    }
}
