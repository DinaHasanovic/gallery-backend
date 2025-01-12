using AppBackEnd.Data;
using AppBackEnd.DTO;
using AutoMapper;

namespace AppBackEnd.Profiles
{
    public class CityMappingProfile : Profile
    {
        public CityMappingProfile() 
        {
            CreateMap<City, CityResponseDTO>();
            CreateMap<CreateCityRequestDTO, City>();
            CreateMap<UpdateCityRequestDTO, City>();
            
        }
    }
}
