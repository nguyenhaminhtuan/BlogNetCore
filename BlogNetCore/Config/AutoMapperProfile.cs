using AutoMapper;
using BlogNetCore.Common.DTOs;
using BlogNetCore.Models;

namespace BlogNetCore.Config;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Article, ArticleDto>();
        CreateMap<User, AuthorDto>();
        CreateMap<Tag, TagDto>();
        CreateMap<User, ProfileDto>()
            .ForMember(dest => dest.IsVerified,
                opt => opt
                    .MapFrom(src => src.Status != UserStatus.Verifying));
    }
}