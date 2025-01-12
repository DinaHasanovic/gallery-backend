using AppBackEnd.Data;
using AppBackEnd.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBackEnd.Services
{
    public class VisitService : IVisitService
    {
        private readonly DatabaseContext db;

        public VisitService(DatabaseContext db)
        {
            this.db = db;
        }


        public async Task<bool> CreateVisit(JournalistVisit visit)
        {
            Journalist? j = await db.Journalists.Where(u => u.UserId == visit.JournalistId).FirstOrDefaultAsync();
            if (j == null) { return false; }
            Exhibition? e = await db.Exhibitions.Where(e => e.Id == visit.ExhibitionId).FirstOrDefaultAsync();
            if (e == null) { return false; }
            db.Visits.Add(visit);
            await db.SaveChangesAsync();
            return true;

        }

        public async Task<bool> DeleteVisit(int id)
        {
            JournalistVisit? v = await db.Visits.Where( v => v.Id == id).FirstOrDefaultAsync();
            if (v == null) { return false; }
            db.Visits.Remove(v);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<List<JournalistVisit>> GetAll()
        {
            return await db.Visits.Include(v => v.Exhibition).Include(v => v.Journalist).Include(v =>v.Journalist.User).ToListAsync();
        }

        public async Task<JournalistVisit?> GetVisitById(int id)
        {
            return await db.Visits.Where(v => v.Id == id).Include( v => v.Exhibition).Include(v => v.Journalist).Include( v => 
            v.Journalist.User).FirstOrDefaultAsync();
        }

        public async Task<List<JournalistVisit>> GetVisitsByJournalist(int id)
        {
            return await db.Visits.Where(v => v.Journalist.UserId == id).Include(v => v.Exhibition).Include(v => v.Journalist).Include(v =>
        v.Journalist.User).ToListAsync();
        }
    }
}
