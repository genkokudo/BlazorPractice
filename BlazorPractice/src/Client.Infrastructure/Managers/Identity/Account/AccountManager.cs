using BlazorPractice.Application.Requests.Identity;
using BlazorPractice.Client.Infrastructure.Extensions;
using BlazorPractice.Shared.Wrapper;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorPractice.Client.Infrastructure.Managers.Identity.Account
{
    public class AccountManager : IAccountManager
    {
        private readonly HttpClient _httpClient;

        public AccountManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult> ChangePasswordAsync(ChangePasswordRequest model)
        {
            var response = await _httpClient.PutAsJsonAsync(Routes.AccountEndpoints.ChangePassword, model);
            return await response.ToResult();
        }

        public async Task<IResult> UpdateProfileAsync(UpdateProfileRequest model)
        {
            var response = await _httpClient.PutAsJsonAsync(Routes.AccountEndpoints.UpdateProfile, model);
            return await response.ToResult();
        }

        /// <summary>
        /// APIを叩き、サーバのコントローラから写真を取得する
        /// </summary>
        /// <param name="userId">対象ユーザID</param>
        /// <returns></returns>
        public async Task<IResult<string>> GetProfilePictureAsync(string userId)
        {
            var response = await _httpClient.GetAsync(Routes.AccountEndpoints.GetProfilePicture(userId));
            return await response.ToResult<string>();
        }

        /// <summary>
        /// 写真のアップロード処理
        /// APIでサーバのコントローラにアップロード情報を送信する
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId)
        {
            // APIを叩いてコントローラにUploadのリクエストを送る
            // BlazorPractice.Server.Controllers.Identity.AccountController
            var response = await _httpClient.PostAsJsonAsync(Routes.AccountEndpoints.UpdateProfilePicture(userId), request);
            return await response.ToResult<string>();
        }
    }
}