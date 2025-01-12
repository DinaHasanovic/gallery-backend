using AppBackEnd.Data;

namespace AppBackEnd.Interfaces
{
    public interface IArtworkGradeService
    {
        public Task<List<ArtworkGrade>> GetAllGrades();
        public Task<List<ArtworkGrade>> GetAllGradesByArtwork(int artworkId);
        public Task<List<ArtworkGrade>> GetAllGradesByJuryMember(int juryMemberId);
        public Task<ArtworkGrade?> GetArtworkGradeById(int id);
        public Task<bool> CreateGrade(ArtworkGrade grade);
        public Task<bool> UpdateGrade(int artworkGradeId,int newGrade);
        public Task<bool> DeleteGrade(int artworkGradeId);

    }
}
