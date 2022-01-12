namespace BlazorPractice.Application.Requests.Identity
{
    /// <summary>
    /// トークンの更新を行うRequest
    /// </summary>
    public class RefreshTokenRequest
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}