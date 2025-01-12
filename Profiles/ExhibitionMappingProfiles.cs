using AppBackEnd.Data;
using AppBackEnd.DTO;
using AutoMapper;

namespace AppBackEnd.Profiles
{
    public class ExhibitionMappingProfiles : Profile
    {
        public ExhibitionMappingProfiles()
        {
            CreateMap<CreateExhibitionRequestDTO, Exhibition>();
            CreateMap<Exhibition,ExhibitionResponseDTO>();
        }
    }
}
