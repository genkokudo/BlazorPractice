namespace BlazorPractice.Client.Infrastructure.Routes
{
    public static class AccountEndpoints
    {
        public static string Register = "api/identity/account/register";
        public static string ChangePassword = "api/identity/account/changepassword";
        public static string UpdateProfile = "api/identity/account/updateprofile";

        // APIのパスを取得
        // 別プロジェクトの処理を呼び出す
        // BlazorPractice.Server.Controllers.Identity.AccountController

        /// <summary>
        /// 写真URL取得のapiパスを取得する
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetProfilePicture(string userId)
        {
            return $"api/identity/account/profile-picture/{userId}";
        }

        /// <summary>
        /// 写真アップロード時のapiパスを取得する
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string UpdateProfilePicture(string userId)
        {
            return $"api/identity/account/profile-picture/{userId}";
        }
    }
}