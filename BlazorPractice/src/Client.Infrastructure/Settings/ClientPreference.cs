using BlazorPractice.Shared.Constants.Localization;
using BlazorPractice.Shared.Settings;
using System.Linq;

namespace BlazorPractice.Client.Infrastructure.Settings
{
    /// <summary>
    /// 表示設定
    /// ユーザのローカルストレージに保存する
    /// </summary>
    public record ClientPreference : IPreference
    {
        /// <summary>
        /// ダークモードか
        /// </summary>
        public bool IsDarkMode { get; set; }

        /// <summary>
        /// ダークモードでない場合、右横書き言語か
        /// 
        /// 使っていない
        /// </summary>
        public bool IsRTL { get; set; }


        public bool IsDrawerOpen { get; set; }
        public string PrimaryColor { get; set; }
        /// <summary>
        /// 表示する言語
        /// 対応言語が1つも設定されていない場合は英語
        /// </summary>
        public string LanguageCode { get; set; } = LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "en-US";
    }
}