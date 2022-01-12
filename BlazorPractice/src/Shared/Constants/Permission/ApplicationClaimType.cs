namespace BlazorPractice.Shared.Constants.Permission
{
    /// <summary>
    /// AuthorizationHandlerContext.User.ClaimsのType
    /// 1種類のみ？
    /// PermissionAuthorizationHandler等でUserのClaimsから権限を探すのに使う
    /// BlazorHeroStateProvider等でユーザにclaimを追加する時に使用する
    /// </summary>
    public static class ApplicationClaimTypes
    {
        public const string Permission = "Permission";
    }
}