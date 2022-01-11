using Microsoft.Extensions.Localization;

namespace BlazorPractice.Server.Localization
{
    /// <summary>
    /// ローカライズサービス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ServerLocalizer<T> where T : class
    {
        public IStringLocalizer<T> Localizer { get; }

        public ServerLocalizer(IStringLocalizer<T> localizer)
        {
            Localizer = localizer;
        }
    }
}
