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
            services.AddApplicationLayer();                 // AutoMapperとMediatRを使用する
            services.AddApplicationServices();              // その他独自のサービスの登録
            services.AddRepositories();                     // 各EntityのRepositoryクラスをサービス登録して、各テーブルの操作を行うクラスをDIできるようにする。
            services.AddExtendedAttributesUnitOfWork();     // UnitOfWorkという実装パターンで、DBの一貫性を保つ
            services.AddSharedInfrastructure(_configuration);   // その他のサービスを登録（時間、メール）
            services.RegisterSwagger();                     // Swaggerを使用する
            services.AddInfrastructureMappings();           // AutoMapperの設定（マッピングが他のアセンブリにあるため）
            services.AddHangfire(x => x.UseSqlServerStorage(_configuration.GetConnectionString("DefaultConnection")));  // Hangfire
            services.AddHangfireServer();                   // 自動化されたタスクまたはcronジョブを管理するための便利なツール、毎日または毎週特定の時間に実行する必要があるサービスメソッドがある場合
            services.AddControllers().AddValidators();      // FluentValidation
            services.AddExtendedAttributesValidators();     // FluentValidationのバリデータをアセンブリから探してすべて登録する
            services.AddExtendedAttributesHandlers();       // アセンブリ内から監査項目を実装したEntityを探してそのRequestHandlerクラスをサービス登録
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
            // UseCors,UseAuthentication,UseAuthorizationは必ずこの順序で呼ばなければならない
            app.UseCors();                                  // CORS：あるオリジンで動いている Web アプリケーションに対して、別のオリジンのサーバーへのアクセスをオリジン間 HTTP リクエストによって許可できる仕組み
            app.UseExceptionHandling(env);                  // 開発環境の場合、例外が発生したらエラーページを表示する（NET6から追加されたErrorBoundary コンポーネントを使った方が良いのでは？）
            app.UseHttpsRedirection();                      // HTTP 要求を HTTPS にリダイレクトする
            app.UseMiddleware<ErrorHandlerMiddleware>();    // エラー発生時にレスポンスに入れるステータスコードを設定する
            app.UseBlazorFrameworkFiles();                  // Blazor WebAssembly フレームワークのファイルをルートパス "/" から提供するように、アプリケーションを設定します。
            app.UseStaticFiles();                           // Server/Files以下を静的ファイルとする
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Files")),
                RequestPath = new PathString("/Files")
            });
            app.UseRequestLocalizationByCulture();          // カルチャ関係だけどよく分からない
            app.UseRouting();                               // ルーティング
            app.UseAuthentication();                        // ユーザーがセキュリティで保護されたリソースにアクセスする前に、ユーザーの認証が試行されます。
            app.UseAuthorization();                         // ユーザーがセキュリティで保護されたリソースにアクセスすることが承認されます。
            app.UseHangfireDashboard("/jobs", new DashboardOptions              // Hangfire（定時実行処理）のダッシュボード
            {
                DashboardTitle = localizer["BlazorHero Jobs"],
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });
            app.UseEndpoints();
            app.ConfigureSwagger();                         // Swaggerの設定
            app.Initialize(_configuration);                 // IDatabaseSeederで初期化（DB初期データがまだない場合にデータを入れる）
        }
    }
}