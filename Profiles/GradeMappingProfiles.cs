using AppBackEnd.Data;
using AppBackEnd.DTO;
using AutoMapper;

namespace AppBackEnd.Profiles
{
    public class GradeMappingProfiles : Profile
    {
        public GradeMappingProfiles()
        {
            CreateMap<CreateGradeRequestDTO,ArtworkGrade>();
            CreateMap< ArtworkGrade,GradeResponseDTO>();
        }
    }
}
