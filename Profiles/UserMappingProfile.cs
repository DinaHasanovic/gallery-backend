using AppBackEnd.Data;
using AppBackEnd.DTO;
using AutoMapper;

namespace AppBackEnd.Profiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile() {
            CreateMap<RegisterUserRequestDTO, User>();
            CreateMap<User, LoginUserResponseDTO>();
            CreateMap<Painter, LoginPainterResponseDTO>();
            CreateMap<Journalist, LoginJournalistResponseDTO>();
            CreateMap<JuryMember, LoginJuryMemberResponseDTO>();
            CreateMap<Painter, ArtworkPainterDTO>();
            CreateMap<User, UserSearchResponseDTO>();
            CreateMap<Journalist,VisitJournalistDTO>();

        }
    }

}
