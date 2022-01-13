using Hangfire.Dashboard;

namespace BlazorPractice.Server.Filters
{
    /// <summary>
    /// UseHangfireDashboardで使用
    /// </summary>
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            //TODO 重要な認証ロジック

            //var httpContext = context.GetHttpContext();

            // すべての認証ユーザーにダッシュボードの閲覧を許可する（危険な可能性あり）。
            //return httpContext.User.Identity.IsAuthenticated;
            //return httpContext.User.IsInRole(Permissions.Hangfire.View);

            return true;
        }
    }
}