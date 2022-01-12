using BlazorPractice.Application.Configurations;
using BlazorPractice.Application.Interfaces.Services.Identity;
using BlazorPractice.Application.Requests.Identity;
using BlazorPractice.Application.Responses.Identity;
using BlazorPractice.Infrastructure.Models.Identity;
using BlazorPractice.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPractice.Infrastructure.Services.Identity
{
    /// <summary>
    /// ユーザ認証関係の処理
    /// </summary>
    public class IdentityService : ITokenService
    {
        private const string InvalidErrorMessage = "Invalid email or password.";      // 使ってない

        private readonly UserManager<BlazorHeroUser> _userManager;          // ユーザ情報に問い合わせる
        private readonly RoleManager<BlazorHeroRole> _roleManager;
        private readonly AppConfiguration _appConfig;
        private readonly SignInManager<BlazorHeroUser> _signInManager;      // 使ってない
        private readonly IStringLocalizer<IdentityService> _localizer;

        public IdentityService(
            UserManager<BlazorHeroUser> userManager, RoleManager<BlazorHeroRole> roleManager,
            IOptions<AppConfiguration> appConfig, SignInManager<BlazorHeroUser> signInManager,
            IStringLocalizer<IdentityService> localizer)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _appConfig = appConfig.Value;
            _signInManager = signInManager;
            _localizer = localizer;
        }

        /// <summary>
        /// IDとパスワードでログイン
        /// </summary>
        /// <param name="model">ログイン入力（メール・パスワード）</param>
        /// <returns></returns>
        public async Task<Result<TokenResponse>> LoginAsync(TokenRequest model)
        {
            // ユーザの存在・状態確認
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["User Not Found."]);
            }
            if (!user.IsActive)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["User Not Active. Please contact the administrator."]);
            }
            if (!user.EmailConfirmed)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["E-Mail not confirmed."]);
            }

            // パスワード認証
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["Invalid Credentials."]);
            }

            // Webトークンを発行する
            user.RefreshToken = GenerateRefreshToken();                 // ランダムなBase64文字列を生成
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);      // 有効期限7日間
            await _userManager.UpdateAsync(user);                       // ユーザの情報に反映させる（詳しくは見てない）

            var token = await GenerateJwtAsync(user);                   // JSON Web Tokenを生成
            var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, UserImageURL = user.ProfilePictureDataUrl };
            return await Result<TokenResponse>.SuccessAsync(response);
        }

        /// <summary>
        /// 新しくトークンを発行する
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model)
        {
            if (model is null)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["Invalid Client Token."]);
            }
            var userPrincipal = GetPrincipalFromExpiredToken(model.Token);  // 期限切れのトークンからプリンシパルを取得
            var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                return await Result<TokenResponse>.FailAsync(_localizer["User Not Found."]);
            if (user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return await Result<TokenResponse>.FailAsync(_localizer["Invalid Client Token."]);
            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));    // JSON Web Tokenを生成：これGenerateJwtAsyncメソッドを使えば良いのでは？
            user.RefreshToken = GenerateRefreshToken();     // ランダムなBase64文字列を生成
            await _userManager.UpdateAsync(user);

            var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
            return await Result<TokenResponse>.SuccessAsync(response);
        }

        /// <summary>
        /// JSON Web Tokenを生成
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<string> GenerateJwtAsync(BlazorHeroUser user)
        {
            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            return token;
        }

        /// <summary>
        /// ユーザ情報からClaimのリストを取得
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<IEnumerable<Claim>> GetClaimsAsync(BlazorHeroUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            var permissionClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
                var thisRole = await _roleManager.FindByNameAsync(role);
                var allPermissionsForThisRoles = await _roleManager.GetClaimsAsync(thisRole);
                permissionClaims.AddRange(allPermissionsForThisRoles);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
            }
            .Union(userClaims)
            .Union(roleClaims)
            .Union(permissionClaims);

            return claims;
        }

        /// <summary>
        /// ランダムなBase64文字列を生成
        /// </summary>
        /// <returns></returns>
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        /// <summary>
        /// デジタル署名とClaimからJSON Web Tokenを生成
        /// </summary>
        /// <param name="signingCredentials">デジタル署名の生成に使用される暗号化キーとセキュリティ アルゴリズム</param>
        /// <param name="claims"></param>
        /// <returns></returns>
        private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
               claims: claims,
               expires: DateTime.UtcNow.AddDays(2),
               signingCredentials: signingCredentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            var encryptedToken = tokenHandler.WriteToken(token);
            return encryptedToken;
        }

        /// <summary>
        /// 期限切れのトークンからプリンシパルを取得
        /// </summary>
        /// <param name="token"></param>
        /// <returns>複数の要求ベース ID をサポートする</returns>
        /// <exception cref="SecurityTokenException"></exception>
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)      // プリンシパルってアプリケーション用のIDだと思えば良い？
        {
            // トークン検証のパラメータを設定
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };

            // 期限切れのトークンからプリンシパルを取得
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            // 取得したトークンがJWTのセキュリティトークンでない場合はエラー
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException(_localizer["Invalid token"]);
            }

            return principal;
        }

        /// <summary>
        /// アプリ設定のSecretと"HS256"によって
        /// デジタル署名を取得
        /// </summary>
        /// <returns></returns>
        private SigningCredentials GetSigningCredentials()
        {
            var secret = Encoding.UTF8.GetBytes(_appConfig.Secret);
            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }
    }
}