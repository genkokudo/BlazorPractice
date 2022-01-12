using BlazorPractice.Application.Interfaces.Common;
using BlazorPractice.Application.Requests.Identity;
using BlazorPractice.Application.Responses.Identity;
using BlazorPractice.Shared.Wrapper;
using System.Threading.Tasks;

namespace BlazorPractice.Application.Interfaces.Services.Identity
{
    // ★トークンベースの認証
    // JSON Web Token（JWT）→署名アルゴリズム、トークン発行者、トークンの有効期限、改ざん防止用の署名
    // JWTは単一のキーに依存する（コンフィグファイルのSecretのこと？）

    // 2者間で情報を安全に送るためにアクセストークンを作成するコンパクトで自己完結型の方法で、RFC 7519として定義されている業界標準の技術。
    // ユーザーが自身の認証で認証サーバーにログインしたとき、Webトークンが返される。
    // 同じユーザーがアプリケーションにAPIコールを行ったとき、アプリケーションはWebトークンを用いてアクセスが保護されたリソースに試みてユーザーの認証を行う。

    /// <summary>
    /// ログインとトークンの更新
    /// </summary>
    public interface ITokenService : IService
    {
        /// <summary>
        /// IDとパスワードでログイン
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Result<TokenResponse>> LoginAsync(TokenRequest model);

        /// <summary>
        /// 現在のトークンから新しいトークンを取得する
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model);
    }
}