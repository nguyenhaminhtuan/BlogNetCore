using Api.Models;
using AutoMapper;

namespace Api.Controllers.DTOs;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Tag, TagDto>();
        CreateMap<Article, ArticleDto>();
        CreateMap<User, AuthorDto>();
        CreateMap<User, ProfileDto>()
            .ForMember(dest => dest.IsVerified,
                opt => opt
                    .MapFrom(src => src.Status != UserStatus.Verifying));
    }
}