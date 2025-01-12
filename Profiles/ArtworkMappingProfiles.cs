using AppBackEnd.Data;
using AppBackEnd.DTO;
using AutoMapper;

namespace AppBackEnd.Profiles
{
    public class ArtworkMappingProfiles : Profile
    {
        public ArtworkMappingProfiles()
        {
            CreateMap<Artwork, ArtworkResponseDTO>();
            CreateMap<CreateArtworkRequestDTO, Artwork>();
            CreateMap<Artwork, ThemeArtworkDTO>();
            CreateMap<Artwork, ExhibitionArtworkDTO>();
        }
    }
}
