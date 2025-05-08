using AuthNexus.Application.Features.Authentication.Dtos;
using AuthNexus.SharedKernel.Models;
using MediatR;

namespace AuthNexus.Application.Features.Authentication.Queries.GetMyProfile
{
    /// <summary>
    /// 获取当前用户个人资料查询
    /// </summary>
    public class GetMyProfileQuery : IRequest<Result<UserProfileDto>>
    {
        // 无需参数，将从当前用户上下文中获取用户ID
    }
}