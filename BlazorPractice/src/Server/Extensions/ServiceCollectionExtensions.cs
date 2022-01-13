using BlazorPractice.Application.Configurations;
using BlazorPractice.Application.Interfaces.Serialization.Options;
using BlazorPractice.Application.Interfaces.Serialization.Serializers;
using BlazorPractice.Application.Interfaces.Serialization.Settings;
using BlazorPractice.Application.Interfaces.Services;
using BlazorPractice.Application.Interfaces.Services.Account;
using BlazorPractice.Application.Interfaces.Services.Identity;
using BlazorPractice.Application.Serialization.JsonConverters;
using BlazorPractice.Application.Serialization.Options;
using BlazorPractice.Application.Serialization.Serializers;
using BlazorPractice.Application.Serialization.Settings;
using BlazorPractice.Infrastructure;
using BlazorPractice.Infrastructure.Contexts;
using BlazorPractice.Infrastructure.Models.Identity;
using BlazorPractice.Infrastructure.Services;
using BlazorPractice.Infrastructure.Services.Identity;
using BlazorPractice.Infrastructure.Shared.Services;
using BlazorPractice.Server.Localization;
using BlazorPractice.Server.Managers.Preferences;
using BlazorPractice.Server.Permission;
using BlazorPractice.Server.Services;
using BlazorPractice.Server.Settings;
using BlazorPractice.Shared.Constants.Localization;
using BlazorPractice.Shared.Constants.Permission;
using BlazorPractice.Shared.Wrapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPractice.Server.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static async Task<IStringLocalizer> GetRegisteredServerLocalizerAsync<T>(this IServiceCollection services) where T : class
        {
            var serviceProvider = services.BuildServiceProvider();
            await SetCultureFromServerPreferenceAsync(serviceProvider);
            var localizer = serviceProvider.GetService<IStringLocalizer<T>>();
            await serviceProvider.DisposeAsync();
            return localizer;
        }

        private static async Task SetCultureFromServerPreferenceAsync(IServiceProvider serviceProvider)
        {
            var storageService = serviceProvider.GetService<ServerPreferenceManager>();
            if (storageService != null)
            {
                // TODO - ServerStorageProviderを実装しなければ、正しく動作しません！
                CultureInfo culture;
                var preference = await storageService.GetPreference() as ServerPreference;
                if (preference != null)
                    culture = new CultureInfo(preference.LanguageCode);
                else
                    culture = new CultureInfo(LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "en-US");
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
        }

        internal static IServiceCollection AddServerLocalization(this IServiceCollection services)
        {
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(ServerLocalizer<>));
            return services;
        }

        internal static AppConfiguration GetApplicationSettings(
           this IServiceCollection services,
           IConfiguration configuration)
        {
            var applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
            services.Configure<AppConfiguration>(applicationSettingsConfiguration);
            return applicationSettingsConfiguration.Get<AppConfiguration>();
        }

        /// <summary>
        /// Swaggerを有効にする
        /// （APIドキュメントの自動生成）
        /// </summary>
        /// <param name="services"></param>
        internal static void RegisterSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(async c =>
            {
                //TODO - Lowercase Swagger Documents
                //c.DocumentFilter<LowercaseDocumentFilter>();
                //Refer - https://gist.github.com/rafalkasa/01d5e3b265e5aa075678e0adfd54e23f

                // include all project's xml comments
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (!assembly.IsDynamic)
                    {
                        var xmlFile = $"{assembly.GetName().Name}.xml";
                        var xmlPath = Path.Combine(baseDirectory, xmlFile);
                        if (File.Exists(xmlPath))
                        {
                            c.IncludeXmlComments(xmlPath);
                        }
                    }
                }

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "BlazorPractice",
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                var localizer = await GetRegisteredServerLocalizerAsync<ServerCommonResources>(services);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = localizer["Input your Bearer token in this format - Bearer {your token here} to access this API"],
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        }, new List<string>()
                    },
                });
            });
        }

        /// <summary>
        /// Jsonシリアライザをサービス登録する
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        internal static IServiceCollection AddSerialization(this IServiceCollection services)
        {
            services
                .AddScoped<IJsonSerializerOptions, SystemTextJsonOptions>()
                .Configure<SystemTextJsonOptions>(configureOptions =>
                {
                    if (!configureOptions.JsonSerializerOptions.Converters.Any(c => c.GetType() == typeof(TimespanJsonConverter)))
                        configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                });
            services.AddScoped<IJsonSerializerSettings, NewtonsoftJsonSettings>();

            services.AddScoped<IJsonSerializer, SystemTextJsonSerializer>(); // 変更可能
            return services;
        }

        /// <summary>
        /// データベースをサービス登録する
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        internal static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
            => services
                .AddDbContext<BlazorHeroContext>(options => options
                    .UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
            .AddTransient<IDatabaseSeeder, DatabaseSeeder>();

        /// <summary>
        /// 現在のユーザのClainやIDを取得するアクセサを登録する
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        internal static IServiceCollection AddCurrentUserService(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();  // IHttpContextAccessorについてデフォルトの実装を追加
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            return services;
        }

        /// <summary>
        /// ポリシー判定サービスやパスワードポリシーの設定
        /// カスタムしたユーザと権限情報の使用設定
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        internal static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services
                .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>() // カスタム認可ポリシー プロバイダー：ポリシー名を指定して、その認可ポリシーを取得する
                .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>()     // ユーザのClaimから必要な権限があるかを探して、許可の判定をする（いつ行われるの？）
                .AddIdentity<BlazorHeroUser, BlazorHeroRole>(options =>
                {
                    // パスワードポリシーを設定
                    options.Password.RequiredLength = 6;                // 6文字
                    options.Password.RequireDigit = false;              // 数字を混ぜない
                    options.Password.RequireLowercase = false;          // 小文字が含まれなくてよい
                    options.Password.RequireNonAlphanumeric = false;    // 非英数字を含まなくてもよい
                    options.Password.RequireUppercase = false;          // 大文字を含めなくても良い

                    // ユーザの設定
                    options.User.RequireUniqueEmail = true;             // Emailの重複禁止
                })
                .AddEntityFrameworkStores<BlazorHeroContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        /// <summary>
        /// その他のサービスを登録
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        internal static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IDateTimeService, SystemDateTimeService>();   // 使用する時間を統一する
            services.Configure<MailConfiguration>(configuration.GetSection("MailConfiguration"));
            services.AddTransient<IMailService, SMTPMailService>();     // SMTPでメールを送信する
            return services;
        }

        /// <summary>
        /// その他独自のサービスの登録
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        internal static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IRoleClaimService, RoleClaimService>();   // RoleClaimテーブルに問い合わせる
            services.AddTransient<ITokenService, IdentityService>();        // ユーザ認証関係の処理
            services.AddTransient<IRoleService, RoleService>();             // ロール情報を問い合わせる
            services.AddTransient<IAccountService, AccountService>();       // アカウントの操作関係の処理
            services.AddTransient<IUserService, UserService>();             // ユーザ情報に関する処理
            services.AddTransient<IChatService, ChatService>();             // チャットに関する処理
            services.AddTransient<IUploadService, UploadService>();         // ファイルのアップロード処理
            services.AddTransient<IAuditService, AuditService>();           // 監査（更新テーブル、項目、値の履歴）データを取得、出力する
            services.AddScoped<IExcelService, ExcelService>();              // Excel出力処理
            return services;
        }

        // ここで設定しているポリシーは、[Authorize(Policy = Permissions.Products.View)]みたいに使用する
        /// <summary>
        /// Bearer認証を設定する
        /// Permissionsクラスに設定している権限を全てポリシーに設定する
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        internal static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services, AppConfiguration config)
        {
            // この辺でBearer認証を設定する
            var key = Encoding.ASCII.GetBytes(config.Secret);
            services
                .AddAuthentication(authentication =>
                {
                    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;  // "Bearer"認証
                    authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;     // "Bearer"認証
                })
                .AddJwtBearer(async bearer =>
                {
                    bearer.RequireHttpsMetadata = false;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RoleClaimType = ClaimTypes.Role,
                        ClockSkew = TimeSpan.Zero
                    };

                    var localizer = await GetRegisteredServerLocalizerAsync<ServerCommonResources>(services);

                    bearer.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = c =>
                        {
                            if (c.Exception is SecurityTokenExpiredException)
                            {
                                c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                c.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(Result.Fail(localizer["The Token is expired."]));
                                return c.Response.WriteAsync(result);
                            }
                            else
                            {
                                c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                c.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(Result.Fail(localizer["An unhandled error has occurred."]));
                                return c.Response.WriteAsync(result);
                            }
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(Result.Fail(localizer["You are not Authorized."]));
                                return context.Response.WriteAsync(result);
                            }

                            return Task.CompletedTask;
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(Result.Fail(localizer["You are not authorized to access this resource."]));
                            return context.Response.WriteAsync(result);
                        },
                    };
                });

            // Permissionsクラスに必要な権限/役割を定数で保存しているので、それを取得
            // 全てAddPolicyする
            services.AddAuthorization(options =>
            {
                // Public, Static, 階層上位のPublic/Static
                foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
                {
                    var propertyValue = prop.GetValue(null);
                    if (propertyValue is not null)
                    {
                        // ポリシー名と権限判定ロジックを指定する（全部RequireClaimにする）
                        // ここで設定すると、IAuthorizationHandlerによって、要件が満たされているかどうかがチェックされる（AddIdentityで設定してるやつ）
                        options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(ApplicationClaimTypes.Permission, propertyValue.ToString()));
                    }
                }
            });
            return services;
        }
    }
}