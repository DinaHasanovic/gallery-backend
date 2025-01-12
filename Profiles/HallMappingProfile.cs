using AppBackEnd.Data;
using AppBackEnd.DTO;
using AutoMapper;

namespace AppBackEnd.Profiles
{
    public class HallMappingProfile : Profile
    {
        public HallMappingProfile()
        {
            CreateMap<Hall, HallResponseDTO>();
            CreateMap<CreateHallRequestDTO, Hall>();
            CreateMap<UpdateHallRequestDTO, Hall>();
            CreateMap<Hall, ArtworkHallDTO>();

        }
    }
}
