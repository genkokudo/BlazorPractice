using AutoMapper;
using BlazorPractice.Application.Interfaces.Chat;
using BlazorPractice.Application.Models.Chat;
using BlazorPractice.Infrastructure.Models.Identity;

namespace BlazorPractice.Infrastructure.Mappings
{
    public class ChatHistoryProfile : Profile
    {
        public ChatHistoryProfile()
        {
            CreateMap<ChatHistory<IChatUser>, ChatHistory<BlazorHeroUser>>().ReverseMap();
        }
    }
}