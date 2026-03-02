using AppBackEnd.Data;
using AppBackEnd.DTO;
using AutoMapper;

namespace AppBackEnd.Profiles
{
    public class GradeMappingProfiles : Profile
    { //Ovaj CreateMap omogućava konverziju podataka iz objekta tipa CreateGradeRequestDTO u entitet ArtworkGrade.
        public GradeMappingProfiles()
        {
            CreateMap<CreateGradeRequestDTO,ArtworkGrade>();
            CreateMap< ArtworkGrade,GradeResponseDTO>();
        }
    }
}
