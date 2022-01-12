using Microsoft.AspNetCore.Authorization;

namespace BlazorPractice.Server.Permission
{
    /// <summary>
    /// 必要な権限チェックを行うときの許可の条件
    /// </summary>
    internal class PermissionRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// この文字列を条件にUserのClaimを検索する
        /// あれば認証が成功する
        /// "Permission"で始まる？
        /// </summary>
        public string Permission { get; private set; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}