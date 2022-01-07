using Blazored.LocalStorage;
using BlazorPractice.Shared.Constants.Permission;
using BlazorPractice.Shared.Constants.Storage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

// ローカルストレージに認証情報を保持することで画面をリロードしてもトークンが期限切れになるまで再度ログインする必要がなくなります。
namespace BlazorPractice.Client.Infrastructure.Authentication
{
    // AuthenticationStateProvider は、認証状態を取得するために AuthorizeView コンポーネントと CascadingAuthenticationState コンポーネントによって使用される基となるサービスです。
    // https://qiita.com/nobu17/items/91c96ede1bd043fe1373
    // 要するに、SPAでどうやって認証して画面を変化させるか。

    // 認証すると、以下で囲った部分が表示される
    // <AuthorizeView><Authorized></Authorized></AuthorizeView>

    // @attribute [Authorize] を使うと、ページ全体を認証時にのみ表示を行う

    // 認証状態が変化した場合に NotifyAuthenticationStateChanged で通知して画面表示を変えるというのが基本。

    // ---- 関連情報 ----
    // 未ログイン時にログインページにリダイレクトする
    // https://qiita.com/nobu17/items/d43b18b8d42e7d0b4535

    /// <summary>
    /// 現在のユーザーの認証状態に関する情報を提供します。
    /// </summary>
    public class BlazorHeroStateProvider : AuthenticationStateProvider  // 標準ライブラリのクラスを継承
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public BlazorHeroStateProvider(
            HttpClient httpClient,
            ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        /// <summary>
        /// ユーザを認証済みにし、Claimにユーザ名を持たせる
        /// </summary>
        /// <param name="userName"></param>
        public void MarkUserAsAuthenticated(string userName)    // userNameとあるが、実際はログインに使っているEmailアドレス
        {
            var authenticatedUser = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userName)
                }, "apiauth"));

            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

            // ログイン/ログアウト等で認証状態が変化した場合にこのメソッドを呼び出すことで状態変更がAuthorizeViewなどに通知されます。
            NotifyAuthenticationStateChanged(authState);
        }

        // 仕組みが分からない。何故これでログアウトしたことになるのか？
        /// <summary>
        /// ユーザの状態をログアウト状態にする
        /// </summary>
        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());  // ユーザ名は持たせない
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));

            // 認証状態が変更されたことを通知する
            NotifyAuthenticationStateChanged(authState);
        }

        /// <summary>
        /// NavMenuから呼ぶ
        /// ローカルストレージから認証しているユーザ名を取得する
        /// </summary>
        /// <returns>認証しているユーザ名</returns>
        public async Task<ClaimsPrincipal> GetAuthenticationStateProviderUserAsync()
        {
            var state = await GetAuthenticationStateAsync();    // 認証状態を取得
            var authenticationStateProviderUser = state.User;
            return authenticationStateProviderUser;
        }

        /// <summary>現在認証しているユーザ</summary>
        public ClaimsPrincipal AuthenticationStateUser { get; set; }

        /// <summary>
        /// 認証が必要となったタイミングでこのメソッドが呼び出されます。
        /// 戻り値に返されるAuthenticationStateの値により認証されたかどうかを判断します。
        /// 
        /// ローカルストレージから認証情報を取得します。
        /// </summary>
        /// <returns></returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // 元の処理より、ローカルストレージからトークンを取得することによって判別するように変更
            var savedToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
            if (string.IsNullOrWhiteSpace(savedToken))
            {
                // 無ければ未ログインの情報を返す
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // ローカルストレージに認証トークンがあればBearer認証とする
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);

            // ローカルストレージの認証トークンはJWTなので、解析してClaimリストにする
            var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(GetClaimsFromJwt(savedToken), "jwt")));
            AuthenticationStateUser = state.User;

            // 認証情報を返す
            return state;
        }

        /// <summary>
        /// JSON Web TokenからClaimのリストを取得する
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns></returns>
        private IEnumerable<Claim> GetClaimsFromJwt(string jwt) // jwt:JSON Web Token
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs != null)
            {
                keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);

                if (roles != null)
                {
                    if (roles.ToString().Trim().StartsWith("["))
                    {
                        var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                        claims.AddRange(parsedRoles.Select(role => new Claim(ClaimTypes.Role, role)));
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                    }

                    keyValuePairs.Remove(ClaimTypes.Role);
                }

                keyValuePairs.TryGetValue(ApplicationClaimTypes.Permission, out var permissions);
                if (permissions != null)
                {
                    if (permissions.ToString().Trim().StartsWith("["))
                    {
                        var parsedPermissions = JsonSerializer.Deserialize<string[]>(permissions.ToString());
                        claims.AddRange(parsedPermissions.Select(permission => new Claim(ApplicationClaimTypes.Permission, permission)));
                    }
                    else
                    {
                        claims.Add(new Claim(ApplicationClaimTypes.Permission, permissions.ToString()));
                    }
                    keyValuePairs.Remove(ApplicationClaimTypes.Permission);
                }

                claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
            }
            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            return Convert.FromBase64String(base64);
        }
    }
}