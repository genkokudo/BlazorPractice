using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BlazorPractice.Server.Extensions
{
    internal static class HostBuilderExtensions
    {
        /// <summary>
        /// ログ設定
        /// Program.csで呼ぶ
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        internal static IHostBuilder UseSerilog(this IHostBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
            SerilogHostBuilderExtensions.UseSerilog(builder);
            return builder;
        }
    }
}