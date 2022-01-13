using BlazorPractice.Application.Interfaces.Repositories;
using BlazorPractice.Application.Interfaces.Serialization.Serializers;
using BlazorPractice.Application.Interfaces.Services.Storage;
using BlazorPractice.Application.Interfaces.Services.Storage.Provider;
using BlazorPractice.Application.Serialization.JsonConverters;
using BlazorPractice.Application.Serialization.Options;
using BlazorPractice.Application.Serialization.Serializers;
using BlazorPractice.Infrastructure.Repositories;
using BlazorPractice.Infrastructure.Services.Storage;
using BlazorPractice.Infrastructure.Services.Storage.Provider;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace BlazorPractice.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// AutoMapperの設定だと思うが・・・
        /// Infrastructureという外のプロジェクトを参照しているから必要なのかな？
        /// </summary>
        /// <param name="services"></param>
        public static void AddInfrastructureMappings(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// リポジトリと作業単位のパターンで必要なクラスをサービス登録する
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                .AddTransient(typeof(IRepositoryAsync<,>), typeof(RepositoryAsync<,>))  // 型引数でEntityを指定することで、DbContextに対するCRUDを統一する
                                                                                        // 各Entityに対するRepositoryをサービス登録する
                .AddTransient<IProductRepository, ProductRepository>()
                .AddTransient<IBrandRepository, BrandRepository>()
                .AddTransient<IDocumentRepository, DocumentRepository>()
                .AddTransient<IDocumentTypeRepository, DocumentTypeRepository>()

                .AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
        }

        /// <summary>
        /// UnitOfWorkによって、複数リポジトリの更新不整合を防止する
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddExtendedAttributesUnitOfWork(this IServiceCollection services)
        {
            return services
                .AddTransient(typeof(IExtendedAttributeUnitOfWork<,,>), typeof(ExtendedAttributeUnitOfWork<,,>));
        }

        public static IServiceCollection AddServerStorage(this IServiceCollection services)
            => AddServerStorage(services, null);

        public static IServiceCollection AddServerStorage(this IServiceCollection services, Action<SystemTextJsonOptions> configure)
        {
            return services
                .AddScoped<IJsonSerializer, SystemTextJsonSerializer>()
                .AddScoped<IStorageProvider, ServerStorageProvider>()
                .AddScoped<IServerStorageService, ServerStorageService>()
                .AddScoped<ISyncServerStorageService, ServerStorageService>()
                .Configure<SystemTextJsonOptions>(configureOptions =>
                {
                    configure?.Invoke(configureOptions);
                    if (!configureOptions.JsonSerializerOptions.Converters.Any(c => c.GetType() == typeof(TimespanJsonConverter)))
                        configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                });
        }
    }
}