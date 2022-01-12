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
    /// �T�[�o�v���O�����̐ݒ�
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly IConfiguration _configuration;

        // ���̃��\�b�h�̓����^�C������Ăяo�����B�R���e�i�ɃT�[�r�X��ǉ�����ɂ́A���̃��\�b�h���g�p���܂��B
        // �A�v���P�[�V�����̐ݒ���@�̏ڍׂɂ��� https://go.microsoft.com/fwlink/?LinkID=398940

        public void ConfigureServices(IServiceCollection services)
        {
            // ServiceCollectionExtensions�ɂ������g�����\�b�h���쐬���Ă���̂ŁA��������Q�Ƃ��邱��

            services.AddCors();         // �N���X�I���W���v�� (CORS) ��L���ɂ���
            services.AddSignalR();      // SignalR���g�p����
            services.AddLocalization(options => // ���[�J���C�Y�t�@�C���̃p�X���w�肷��
            {
                options.ResourcesPath = "Resources";
            });
            services.AddCurrentUserService();               // ���݂̃��[�U��Clain��ID���擾����A�N�Z�T���T�[�r�X�o�^����
            services.AddSerialization();                    // Json�V���A���C�U���T�[�r�X�o�^����
            services.AddDatabase(_configuration);           // DB��o�^����
            services.AddServerStorage(); //TODO - ���������삳���邽�߂ɂ́AServerStorageProvider����������K�v������܂��I
            services.AddScoped<ServerPreferenceManager>();  // �T�[�o���̐ݒ�Ǘ���o�^����
            services.AddServerLocalization();               // ���[�J���C�Y�T�[�r�X��o�^����
            services.AddIdentity();                         // �|���V�[����T�[�r�X��p�X���[�h�|���V�[�̐ݒ�A�J�X�^���������[�U�ƌ������
            services.AddJwtAuthentication(services.GetApplicationSettings(_configuration)); // �|���V�[�̎�ނƔ�����@�i����̔N��ȉ��͋֎~�݂����Ȃ́j��o�^
            services.AddApplicationLayer();
            services.AddApplicationServices();
            services.AddRepositories();
            services.AddExtendedAttributesUnitOfWork();     // UnitOfWork�Ƃ��������p�^�[���ŁADB�̈�ѐ���ۂ�
            services.AddSharedInfrastructure(_configuration);
            services.RegisterSwagger();                     // Swagger�𗘗p����
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