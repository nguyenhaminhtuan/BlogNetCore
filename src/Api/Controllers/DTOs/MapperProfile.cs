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
        CreateMap<User, ProfileDto>();
    }
}