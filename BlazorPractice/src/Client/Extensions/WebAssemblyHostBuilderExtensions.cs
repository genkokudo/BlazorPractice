using Blazored.LocalStorage;
using BlazorPractice.Client.Infrastructure.Authentication;
using BlazorPractice.Client.Infrastructure.Managers;
using BlazorPractice.Client.Infrastructure.Managers.ExtendedAttribute;
using BlazorPractice.Client.Infrastructure.Managers.Preferences;
using BlazorPractice.Domain.Entities.ExtendedAttributes;
using BlazorPractice.Domain.Entities.Misc;
using BlazorPractice.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace BlazorPractice.Client.Extensions
{
    /// <summary>
    /// WebAssemblyHostBuilderを拡張することで
    /// 今までStartUpで行なっていた必要な初期設定をここで行う
    /// </summary>
    public static class WebAssemblyHostBuilderExtensions
    {
        private const string ClientName = "BlazorHero.API";

        public static WebAssemblyHostBuilder AddRootComponents(this WebAssemblyHostBuilder builder)
        {
            builder.RootComponents.Add<App>("#app");

            return builder;
        }

        /// <summary>
        /// Mainから呼び出す
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
        {
            builder
                .Services
                .AddLocalization(options =>
                {
                    options.ResourcesPath = "Resources";    // 多言語ファイルの場所を設定
                })
                .AddAuthorizationCore(options =>            // razor pageで @attribute [Authorize] を使用するのに必要。
                {
                    RegisterPermissionClaims(options);      // システムに定義してある権限をAuthorizationOptionsにすべて登録する
                })
                .AddBlazoredLocalStorage()                  // ローカルストレージを使って、ユーザ設定を保存する
                .AddMudServices(configuration =>            // MudBlazorを使う
                {
                    // ちゃんと見てないからよく分からない
                    configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                    configuration.SnackbarConfiguration.HideTransitionDuration = 100;
                    configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
                    configuration.SnackbarConfiguration.VisibleStateDuration = 3000;
                    configuration.SnackbarConfiguration.ShowCloseIcon = false;
                })
                .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())     // AutoMapperを使う
                .AddScoped<ClientPreferenceManager>()                       // IManagerでないものはここで登録、IPreferenceManagerはサーバでも使うのでIManagerにしてクライアント側に登録しない（多分）
                .AddScoped<BlazorHeroStateProvider>()                       // ????
                .AddScoped<AuthenticationStateProvider, BlazorHeroStateProvider>()
                .AddManagers()      // Client.InfrastructureのIManager以下の全サービスを登録
                .AddExtendedAttributeManagers()
                .AddTransient<AuthenticationHeaderHandler>()
                .AddScoped(sp => sp
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient(ClientName).EnableIntercept(sp))
                .AddHttpClient(ClientName, client =>
                {
                    client.DefaultRequestHeaders.AcceptLanguage.Clear();
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
                    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                })
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();
            builder.Services.AddHttpClientInterceptor();
            return builder;
        }

        /// <summary>
        /// 管理処理関係のインタフェースは全てIManagerを継承しているので、
        /// アセンブリからIManagerを継承したインタフェースを実装したクラス（抽象以外）を検索して纏めてサービスに登録する
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddManagers(this IServiceCollection services)
        {
            var managers = typeof(IManager);

            var types = managers
                .Assembly
                .GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Select(t => new
                {
                    Service = t.GetInterface($"I{t.Name}"),
                    Implementation = t
                })
                .Where(t => t.Service != null);

            foreach (var type in types)
            {
                if (managers.IsAssignableFrom(type.Service))
                {
                    services.AddTransient(type.Service, type.Implementation);
                }
            }

            return services;
        }

        public static IServiceCollection AddExtendedAttributeManagers(this IServiceCollection services)
        {
            //TODO - add managers with reflection!

            return services
                .AddTransient(typeof(IExtendedAttributeManager<int, int, Document, DocumentExtendedAttribute>), typeof(ExtendedAttributeManager<int, int, Document, DocumentExtendedAttribute>));
        }

        /// <summary>
        /// 定義してある権限をAuthorizationOptionsにすべて登録する
        /// </summary>
        /// <param name="options"></param>
        private static void RegisterPermissionClaims(AuthorizationOptions options)
        {
            foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                {
                    options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(ApplicationClaimTypes.Permission, propertyValue.ToString()));
                }
            }
        }
    }
}