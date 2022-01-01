using BlazorPractice.Application.Interfaces.Common;

namespace BlazorPractice.Application.Interfaces.Services
{
    public interface ICurrentUserService : IService
    {
        string UserId { get; }
    }
}