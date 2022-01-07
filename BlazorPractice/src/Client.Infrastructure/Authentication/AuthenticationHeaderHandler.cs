using Blazored.LocalStorage;
using BlazorPractice.Shared.Constants.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorPractice.Client.Infrastructure.Authentication
{
    /// <summary>
    /// 内部ハンドラーと呼ばれる、別のハンドラーへ HTTP 応答メッセージの処理をデリゲートする HTTP ハンドラーの型。
    /// DelegatingHandlerはRequestとResponseの間に処理を入れるので、Controllerに処理を渡さずにResponseすることも可能、Request内容を変更することも可能
    /// 
    /// ローカルストレージに認証トークンがあれば、Bearer認証にするように変更する
    /// </summary>
    public class AuthenticationHeaderHandler : DelegatingHandler    // 標準ライブラリを継承
    {
        private readonly ILocalStorageService _localStorage;

        public AuthenticationHeaderHandler(ILocalStorageService localStorage)
            => _localStorage = localStorage;

        /// <summary>
        /// 非同期操作としてサーバーに送信するように HTTP 要求を内部ハンドラーに送信します。
        /// </summary>
        /// <param name="request">サーバーに送信する HTTP 要求メッセージ。</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization?.Scheme != "Bearer")  // Bearer認証（OAuth 2.0とか）ではない場合
            {
                // ローカルストレージに認証トークンがあれば、Bearer認証にする
                var savedToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);

                if (!string.IsNullOrWhiteSpace(savedToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}