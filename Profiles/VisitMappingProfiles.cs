using AppBackEnd.Data;
using AppBackEnd.DTO;
using AutoMapper;

namespace AppBackEnd.Profiles
{
    public class VisitMappingProfiles : Profile
    {
        public VisitMappingProfiles()
        {
            CreateMap<JournalistVisit,VisitResponseDTO>();
            CreateMap<CreateVisitRequestDTO, JournalistVisit>();
        }
    }
}
