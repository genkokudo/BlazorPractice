using BlazorPractice.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPractice.Server.Permission
{
    /// <summary>
    /// ユーザのClaimから必要な権限があるかを探して、許可の判定をする
    /// </summary>
    internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        public PermissionAuthorizationHandler()
        { }

        /// <summary>
        /// Baseの処理は呼ばない
        /// </summary>
        /// <param name="context">認証におけるコンテキスト</param>
        /// <param name="requirement">必要な権限</param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null)
            {
                // ユーザ情報が送られてこなければそのまま
                await Task.CompletedTask;
            }

            // requirementの条件に合ったClaimを探す
            var permissions = context.User.Claims.Where(x => x.Type == ApplicationClaimTypes.Permission &&
                                                                x.Value == requirement.Permission &&
                                                                x.Issuer == "LOCAL AUTHORITY");     // "LOCAL AUTHORITY"はここでしか使われていない
            // 見つかったら認証成功
            if (permissions.Any())
            {
                context.Succeed(requirement);
                await Task.CompletedTask;
            }
        }
    }
}