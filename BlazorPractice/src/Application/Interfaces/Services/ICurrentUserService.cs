using BlazorPractice.Application.Interfaces.Common;

namespace BlazorPractice.Application.Interfaces.Services
{
    /// <summary>
    /// 現在のユーザのClainやIDを取得するアクセサ
    /// </summary>
    public interface ICurrentUserService : IService
    {
        string UserId { get; }
    }
}