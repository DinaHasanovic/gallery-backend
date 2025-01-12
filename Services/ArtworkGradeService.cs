using AppBackEnd.Data;
using AppBackEnd.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace AppBackEnd.Services
{
    public class ArtworkGradeService : IArtworkGradeService
    {
        private readonly DatabaseContext db;

        public ArtworkGradeService(DatabaseContext db)
        {
            this.db = db;
        }


        public async Task<bool> CreateGrade(ArtworkGrade grade)
        {
            try
            {
                db.Grades.Add(grade);
                await db.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteGrade(int artworkGradeId)
        {
            try
            {
                ArtworkGrade? grade = await db.Grades.FindAsync(artworkGradeId);
                if (grade == null)
                {
                    return false;
                }
                db.Grades.Remove(grade);
                await db.SaveChangesAsync();
                return true;

            }
            catch (Exception ex) { 
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<List<ArtworkGrade>> GetAllGrades()
        {
            return await db.Grades.Include( g => g.JuryMember ).Include(g => g.Artwork).ToListAsync();
        }

        public async Task<List<ArtworkGrade>> GetAllGradesByArtwork(int artworkId)
        {
            return await db.Grades.Where( g => g.ArtworkId == artworkId ).Include( g => g.JuryMember).ToListAsync();
        }

        public async Task<List<ArtworkGrade>> GetAllGradesByJuryMember(int juryMemberId)
        {
            return await db.Grades.Where(g => g.JuryMemberId == juryMemberId).Include(g => g.Artwork).ToListAsync();

        }

        public async Task<ArtworkGrade?> GetArtworkGradeById(int id)
        {
            return await db.Grades.Where(g => g.Id == id).Include( g => g.Artwork).Include( g => g.JuryMember).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateGrade(int artworkGradeId, int newGrade)
        {
            try
            {

                ArtworkGrade? grade = await db.Grades.Where(g => g.Id == artworkGradeId).FirstOrDefaultAsync();
                if (grade == null)
                {
                    return false;
                }
                else
                {
                    grade.Points = newGrade;
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex) { 
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
