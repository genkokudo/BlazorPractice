using BlazorPractice.Client.Extensions;
using BlazorPractice.Client.Infrastructure.Managers.Preferences;
using BlazorPractice.Client.Infrastructure.Settings;
using BlazorPractice.Shared.Constants.Localization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPractice.Client
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder
                          .CreateDefault(args)
                          .AddRootComponents()          // 拡張メソッド："#app"を追加する処理
                          .AddClientServices();         // 拡張メソッド：StartUpの代わり
            var host = builder.Build();

            // 上でサービスの追加が完了しているので、取り出して初期処理をする
            // ユーザのブラウザのローカルストレージから、このアプリケーションの表示設定を取得
            var storageService = host.Services.GetRequiredService<ClientPreferenceManager>();
            if (storageService != null)
            {
                CultureInfo culture;
                var preference = await storageService.GetPreference() as ClientPreference;
                if (preference != null)
                    culture = new CultureInfo(preference.LanguageCode);
                else
                    culture = new CultureInfo(LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "en-US");
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }

            await builder.Build().RunAsync();
        }
    }
}