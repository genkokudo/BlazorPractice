using BlazorPractice.Application.Extensions;
using BlazorPractice.Infrastructure.Extensions;
using BlazorPractice.Server.Extensions;
using BlazorPractice.Server.Filters;
using BlazorPractice.Server.Managers.Preferences;
using BlazorPractice.Server.Middlewares;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using System.IO;

namespace BlazorPractice.Server
{
    /// <summary>
    /// サーバプログラムの設定
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly IConfiguration _configuration;

        // このメソッドはランタイムから呼び出される。コンテナにサービスを追加するには、このメソッドを使用します。
        // アプリケーションの設定方法の詳細について https://go.microsoft.com/fwlink/?LinkID=398940

        public void ConfigureServices(IServiceCollection services)
        {
            // ServiceCollectionExtensionsにいくつか拡張メソッドを作成しているので、そちらも参照すること

            services.AddCors();         // クロスオリジン要求 (CORS) を有効にする
            services.AddSignalR();      // SignalRを使用する
            services.AddLocalization(options => // ローカライズファイルのパスを指定する
            {
                options.ResourcesPath = "Resources";
            });
            services.AddCurrentUserService();               // 現在のユーザのClainやIDを取得するアクセサをサービス登録する
            services.AddSerialization();                    // Jsonシリアライザをサービス登録する
            services.AddDatabase(_configuration);           // DBを登録する
            services.AddServerStorage(); //TODO - 正しく動作させるためには、ServerStorageProviderを実装する必要があります！
            services.AddScoped<ServerPreferenceManager>();  // サーバ側の設定管理を登録する
            services.AddServerLocalization();               // ローカライズサービスを登録する
            services.AddIdentity();                         // ポリシー判定サービスやパスワードポリシーの設定、カスタムしたユーザと権限情報
            services.AddJwtAuthentication(services.GetApplicationSettings(_configuration)); // ポリシーの種類と判定方法（特定の年齢以下は禁止みたいなの）を登録
            services.AddApplicationLayer();
            services.AddApplicationServices();
            services.AddRepositories();
            services.AddExtendedAttributesUnitOfWork();     // UnitOfWorkという実装パターンで、DBの一貫性を保つ
            services.AddSharedInfrastructure(_configuration);
            services.RegisterSwagger();                     // Swaggerを利用する
            services.AddInfrastructureMappings();
            services.AddHangfire(x => x.UseSqlServerStorage(_configuration.GetConnectionString("DefaultConnection")));
            services.AddHangfireServer();
            services.AddControllers().AddValidators();
            services.AddExtendedAttributesValidators();
            services.AddExtendedAttributesHandlers();
            services.AddRazorPages();
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });
            services.AddLazyCache();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IStringLocalizer<Startup> localizer)
        {
            app.UseCors();
            app.UseExceptionHandling(env);
            app.UseHttpsRedirection();
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Files")),
                RequestPath = new PathString("/Files")
            });
            app.UseRequestLocalizationByCulture();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHangfireDashboard("/jobs", new DashboardOptions
            {
                DashboardTitle = localizer["BlazorHero Jobs"],
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });
            app.UseEndpoints();
            app.ConfigureSwagger();
            app.Initialize(_configuration);
        }
    }
}