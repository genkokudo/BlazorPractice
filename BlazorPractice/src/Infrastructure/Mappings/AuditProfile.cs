using AutoMapper;
using BlazorPractice.Application.Responses.Audit;
using BlazorPractice.Infrastructure.Models.Audit;

namespace BlazorPractice.Infrastructure.Mappings
{
    public class AuditProfile : Profile
    {
        public AuditProfile()
        {
            CreateMap<AuditResponse, Audit>().ReverseMap();
        }
    }
}