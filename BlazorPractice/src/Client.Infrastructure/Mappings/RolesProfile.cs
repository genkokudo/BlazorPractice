using AutoMapper;
using BlazorPractice.Application.Requests.Identity;
using BlazorPractice.Application.Responses.Identity;

namespace BlazorPractice.Client.Infrastructure.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<PermissionResponse, PermissionRequest>().ReverseMap();
            CreateMap<RoleClaimResponse, RoleClaimRequest>().ReverseMap();
        }
    }
}