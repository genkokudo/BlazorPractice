using BlazorPractice.Application.Requests.Identity;
using BlazorPractice.Shared.Wrapper;
using System.Threading.Tasks;

namespace BlazorPractice.Client.Infrastructure.Managers.Identity.Account
{
    /// <summary>
    /// アカウント・プロフィール画面で実行する処理
    /// </summary>
    public interface IAccountManager : IManager
    {
        Task<IResult> ChangePasswordAsync(ChangePasswordRequest model);

        Task<IResult> UpdateProfileAsync(UpdateProfileRequest model);

        Task<IResult<string>> GetProfilePictureAsync(string userId);

        Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId);
    }
}