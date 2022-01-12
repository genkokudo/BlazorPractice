using BlazorPractice.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace BlazorPractice.Server.Permission
{
    /// <summary>
    /// カスタム認可ポリシー プロバイダー
    /// ポリシー名を指定して、その認可ポリシーを取得する
    /// </summary>
    internal class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

        /// <summary>
        /// "Permission"で始まっているポリシーはPermissionAuthorizationHandlerで判定・取得（多分。違うかもしれない）
        /// それ以外はFallbackPolicyProviderで取得
        /// </summary>
        /// <param name="policyName"></param>
        /// <returns></returns>
        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            // ポリシー名が"Permission"で始まっている場合
            if (policyName.StartsWith(ApplicationClaimTypes.Permission, StringComparison.OrdinalIgnoreCase))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(policyName));      // 必要な許可を設定
                return Task.FromResult(policy.Build());
            }
            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => Task.FromResult<AuthorizationPolicy>(null);
    }
}