using System.ComponentModel.DataAnnotations;

namespace BlazorPractice.Application.Requests.Identity
{
    /// <summary>
    /// パスワード変更用モデル
    /// </summary>
    public class ChangePasswordRequest
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string ConfirmNewPassword { get; set; }
    }
}