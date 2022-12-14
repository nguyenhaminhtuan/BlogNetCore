using Api.Models;
using AutoMapper;

namespace Api.Controllers.DTOs;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Tag, TagDto>();
        CreateMap<Article, ArticleDto>()
            .ForMember(dest => dest.IsArchived,
                src => 
                    src.MapFrom(a => a.Status == ArticleStatus.Archived));
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.IsDisabled,
                src =>
                    src.MapFrom(u => u.Status == UserStatus.Disabled));
        CreateMap<User, ProfileDto>()
            .ForMember(dest => dest.IsDisabled,
                src =>
                    src.MapFrom(u => u.Status == UserStatus.Disabled));
        CreateMap(typeof(PaginatedList<>), typeof(PaginatedDto<>));
        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.Body,
                src => 
                    src.MapFrom(c => !c.IsDeleted ? c.Body : "[DELETED]"));
        CreateMap<Comment, ReplyDto>()
            .ForMember(dest => dest.Body,
                src => 
                    src.MapFrom(c => !c.IsDeleted ? c.Body : "[DELETED]"));
    }
}