using System.ComponentModel.DataAnnotations;

namespace BlazorPractice.Application.Requests.Identity
{
    /// <summary>
    /// ログイン入力
    /// </summary>
    public class TokenRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}