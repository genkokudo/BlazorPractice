using AutoMapper;
using BlazorPractice.Application.Responses.Identity;
using BlazorPractice.Infrastructure.Models.Identity;

namespace BlazorPractice.Infrastructure.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<RoleResponse, BlazorHeroRole>().ReverseMap();
        }
    }
}