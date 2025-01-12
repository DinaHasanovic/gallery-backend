using AppBackEnd.Data;
using AppBackEnd.DTO;
using AutoMapper;

namespace AppBackEnd.Profiles
{
    public class ThemeMappingProfile : Profile
    {
        public ThemeMappingProfile()
        {
            CreateMap<Theme,ThemeResponseDTO>();
            CreateMap<Theme,ThemeDataDTO>();

        }
    }
}
