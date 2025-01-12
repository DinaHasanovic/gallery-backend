using AppBackEnd.Data;
using AppBackEnd.DTO;

namespace AppBackEnd.Interfaces
{
    public interface IArtworkService
    {
        public Task<List<Artwork>> GetAllArtworks();
        public Task<Artwork?> GetArtworkById(int id);
        public Task<List<Artwork>> GetArtworksByArtistId(int artistId);
        public Task<bool> CreateArtwork(Artwork artwork);
        public Task<bool> UpdateArtwork(UpdateArtworkRequestDTO artwork);
        public Task<bool> DeleteArtworkById(int artworkId);
        public Task<Artwork?> GetArtworkWithCode(string code);
        public Task<List<Artwork>> SearchByCode(string code);
        public Task<List<Artwork>> SearchByTitle(string title);
    }
}
