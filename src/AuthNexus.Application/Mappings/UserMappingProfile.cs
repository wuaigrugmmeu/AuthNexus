using AutoMapper;
using AuthNexus.Application.Features.Authentication.Dtos;
using AuthNexus.Domain.Entities;

namespace AuthNexus.Application.Mappings
{
    /// <summary>
    /// 用户映射配置
    /// </summary>
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            // User -> UserProfileDto
            CreateMap<User, UserProfileDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));

            // 添加其他用户相关映射...
        }
    }
}