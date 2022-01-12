using System;

namespace BlazorPractice.Application.Responses.Identity
{
    /// <summary>
    /// 新しくトークンを発行した時のレスポンス
    /// </summary>
    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string UserImageURL { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}