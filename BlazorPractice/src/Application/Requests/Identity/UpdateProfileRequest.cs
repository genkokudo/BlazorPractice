using System.ComponentModel.DataAnnotations;

namespace BlazorPractice.Application.Requests.Identity
{
    /// <summary>
    /// プロフィール更新を行うときにサーバに情報を送る
    /// </summary>
    public class UpdateProfileRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}