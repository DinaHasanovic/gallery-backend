using AppBackEnd.Data;

namespace AppBackEnd.Interfaces
{// upravljanje posetama novinara u aplikaciji. Posete novinara su povezane sa izložbama i omogućavaju novinarima da evidentiraju svoj dolazak 
    public interface IVisitService
    {
        public Task<JournalistVisit?> GetVisitById(int id);
        public Task<List<JournalistVisit>> GetVisitsByJournalist(int id);
        public Task<bool> DeleteVisit(int id);
        public Task<bool> CreateVisit(JournalistVisit visit);
        public Task<List<JournalistVisit>> GetAll();
    }
}
