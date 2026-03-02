using AppBackEnd.Data;
using AppBackEnd.DTO;
using AutoMapper;

namespace AppBackEnd.Profiles
{
    public class ArtworkMappingProfiles : Profile
    {//AutoMapper je biblioteka koja omogućava automatsko mapiranje između različitih objekata (DTO-ova i entiteta)
        public ArtworkMappingProfiles()
        {
            CreateMap<Artwork, ArtworkResponseDTO>();
            CreateMap<CreateArtworkRequestDTO, Artwork>();
            CreateMap<Artwork, ThemeArtworkDTO>();
            CreateMap<Artwork, ExhibitionArtworkDTO>();
        }
    }
}
