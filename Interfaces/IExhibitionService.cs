using AppBackEnd.Data;
using AppBackEnd.DTO;

namespace AppBackEnd.Interfaces
{
    public interface IExhibitionService
    {

        public Task<List<Exhibition>> GetAllExhibitions();
        public Task<Exhibition?> GetExhibitionById(int id);
        public Task<bool> CreateExhibition(Exhibition exhibition);
        public Task<bool> UpdateExhibition(UpdateExhibitionRequestDTO request);
        public Task<bool> DeleteExhibition(int id);
        public Task<bool> AddPictureToExhibition(int exhibitionId, int artworkId);
        public Task<bool> RemovePictureFromExhibition(int exhibitionId, int artworkId);
        public Task<List<Artwork>> GetAllArtworksOfExhibition(int exhibitionId);
        public Task<List<Exhibition>> GetExhibitionsWithTitle(string title);
        public Task<List<Exhibition>> GetExhibitionsWithPlaces(string PTT);
        public Task<List<Exhibition>> GetExhibitionsWithBetweenDates(DateTime? minDate, DateTime? maxDate);
    }
}
