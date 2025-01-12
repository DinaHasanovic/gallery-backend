using AppBackEnd.Data;
using AppBackEnd.DTO;

namespace AppBackEnd.Interfaces
{
    public interface IUserService
    {
        public Task<bool> RegisterUser(User User);
        public Task<bool> TakenEmail(string email);
        public Task<User?> GetUserByEmail(string email);
        public Task<User?> GetUserById(int id);
        public Task<bool> DeleteUserByEmail(string email);
        public Task<bool> DeleteUserById(int id);
        public Task<bool> UpdateUser(UpdateUserRequestDTO User);
        public Task<bool> UpdateUserToJournalist(int id, UpdateToJournalistRequestDTO request);
        public Task<bool> UpdateUserToPainter(int id,UpdateToPainterRequestDTO request);
        public Task<bool> UpdateUserToJury(int id, string JMBG);
        public Task<bool> UpdateUserToAdministrator(int id);
        public Task<bool> UpdateUserToVisitor(int id);
        public string HashPassword(string password);
        public string GenerateToken(User user);
        public Task<List<User>?> GetAllUsers();
        public Task<bool> ChangeFromPainter(int id);
        public Task<bool> ChangeFromJournalist(int id);
        public Task<bool> ChangeFromJuryMember(int id);
        public Task<Painter?> GetPainterData(int id);
        public Task<JuryMember?> GetJuryMemberData(int id);
        public Task<List<User>> SearchUsersByEmail(string email);
        public Task<List<User>> GetUsersByRole(int role);
        public Task<JuryMember?> GetJuryMemberWithJMBG(string JMBG);
        public Task<Painter?> GetPainterWithJMBG(string JMBG);
        public Task<Journalist?> GetJournalistData(int id);
        public Task<User?> GetPaintersUser(int painterId);
    }
}
