using Blazored.LocalStorage;
using BlazorPractice.Client.Infrastructure.Settings;
using BlazorPractice.Shared.Constants.Storage;
using BlazorPractice.Shared.Settings;
using BlazorPractice.Shared.Wrapper;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorPractice.Client.Infrastructure.Managers.Preferences
{
    /// <summary>
    /// アプリケーション設定の管理
    /// Blazored.LocalStorageを使用
    /// </summary>
    public class ClientPreferenceManager : IClientPreferenceManager
    {
        private readonly ILocalStorageService _localStorageService;             // クッキーやローカルストレージを利用して、ログイン情報を保持する
        private readonly IStringLocalizer<ClientPreferenceManager> _localizer;  // 多言語対応

        public ClientPreferenceManager(
            ILocalStorageService localStorageService,
            IStringLocalizer<ClientPreferenceManager> localizer)
        {
            _localStorageService = localStorageService;
            _localizer = localizer;
        }

        /// <summary>
        /// ダークモード表示の切り替え
        /// </summary>
        /// <returns>変更後の値</returns>
        public async Task<bool> ToggleDarkModeAsync()
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                preference.IsDarkMode = !preference.IsDarkMode;
                await SetPreference(preference);    // 設定をローカルストレージに保存する
                return !preference.IsDarkMode;
            }

            return false;
        }

        /// <summary>
        /// ？？？の切り替え
        /// </summary>
        /// <returns>変更後の値</returns>
        public async Task<bool> ToggleLayoutDirection()
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                preference.IsRTL = !preference.IsRTL;
                await SetPreference(preference);
                return preference.IsRTL;
            }
            return false;
        }

        /// <summary>
        /// 表示言語の変更
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns>処理結果</returns>
        public async Task<IResult> ChangeLanguageAsync(string languageCode)
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                preference.LanguageCode = languageCode;
                await SetPreference(preference);
                return new Result
                {
                    Succeeded = true,
                    Messages = new List<string> { _localizer["Client Language has been changed"] }
                };
            }

            return new Result
            {
                Succeeded = false,
                Messages = new List<string> { _localizer["Failed to get client preferences"] }
            };
        }

        /// <summary>
        /// 現在のテーマ設定の色設定を取得
        /// </summary>
        /// <returns>現在のテーマ設定（色など）</returns>
        public async Task<MudTheme> GetCurrentThemeAsync()
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                if (preference.IsDarkMode == true) return BlazorHeroTheme.DarkTheme;
            }
            return BlazorHeroTheme.DefaultTheme;
        }

        /// <summary>
        /// 現在RightToLeftかを取得
        /// ダークモードだとfalse
        /// 
        /// 使っていない
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsRTL()
        {
            var preference = await GetPreference() as ClientPreference;
            if (preference != null)
            {
                if (preference.IsDarkMode == true) return false;
            }
            return preference.IsRTL;
        }

        /// <summary>
        /// ユーザのブラウザのローカルストレージが保持しているこのアプリの表示設定を取得する
        /// 無かったら新規の設定を取得する
        /// </summary>
        /// <returns></returns>
        public async Task<IPreference> GetPreference()
        {
            return await _localStorageService.GetItemAsync<ClientPreference>(StorageConstants.Local.Preference) ?? new ClientPreference();
        }

        /// <summary>
        /// 設定をローカルストレージに保存する
        /// </summary>
        /// <param name="preference"></param>
        /// <returns></returns>
        public async Task SetPreference(IPreference preference)
        {
            await _localStorageService.SetItemAsync(StorageConstants.Local.Preference, preference as ClientPreference);
        }
    }
}