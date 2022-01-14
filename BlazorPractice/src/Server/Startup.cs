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
            services.AddApplicationLayer();                 // AutoMapper��MediatR���g�p����
            services.AddApplicationServices();              // ���̑��Ǝ��̃T�[�r�X�̓o�^
            services.AddRepositories();                     // �eEntity��Repository�N���X���T�[�r�X�o�^���āA�e�e�[�u���̑�����s���N���X��DI�ł���悤�ɂ���B
            services.AddExtendedAttributesUnitOfWork();     // UnitOfWork�Ƃ��������p�^�[���ŁADB�̈�ѐ���ۂ�
            services.AddSharedInfrastructure(_configuration);   // ���̑��̃T�[�r�X��o�^�i���ԁA���[���j
            services.RegisterSwagger();                     // Swagger���g�p����
            services.AddInfrastructureMappings();           // AutoMapper�̐ݒ�i�}�b�s���O�����̃A�Z���u���ɂ��邽�߁j
            services.AddHangfire(x => x.UseSqlServerStorage(_configuration.GetConnectionString("DefaultConnection")));  // Hangfire
            services.AddHangfireServer();                   // ���������ꂽ�^�X�N�܂���cron�W���u���Ǘ����邽�߂֗̕��ȃc�[���A�����܂��͖��T����̎��ԂɎ��s����K�v������T�[�r�X���\�b�h������ꍇ
            services.AddControllers().AddValidators();      // FluentValidation
            services.AddExtendedAttributesValidators();     // FluentValidation�̃o���f�[�^���A�Z���u������T���Ă��ׂēo�^����
            services.AddExtendedAttributesHandlers();       // �A�Z���u��������č����ڂ���������Entity��T���Ă���RequestHandler�N���X���T�[�r�X�o�^
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
            // UseCors,UseAuthentication,UseAuthorization�͕K�����̏����ŌĂ΂Ȃ���΂Ȃ�Ȃ�
            app.UseCors();                                  // CORS�F����I���W���œ����Ă��� Web �A�v���P�[�V�����ɑ΂��āA�ʂ̃I���W���̃T�[�o�[�ւ̃A�N�Z�X���I���W���� HTTP ���N�G�X�g�ɂ���ċ��ł���d�g��
            app.UseExceptionHandling(env);                  // �J�����̏ꍇ�A��O������������G���[�y�[�W��\������iNET6����ǉ����ꂽErrorBoundary �R���|�[�l���g���g���������ǂ��̂ł́H�j
            app.UseHttpsRedirection();                      // HTTP �v���� HTTPS �Ƀ��_�C���N�g����
            app.UseMiddleware<ErrorHandlerMiddleware>();    // �G���[�������Ƀ��X�|���X�ɓ����X�e�[�^�X�R�[�h��ݒ肷��
            app.UseBlazorFrameworkFiles();                  // Blazor WebAssembly �t���[�����[�N�̃t�@�C�������[�g�p�X "/" ����񋟂���悤�ɁA�A�v���P�[�V������ݒ肵�܂��B
            app.UseStaticFiles();                           // Server/Files�ȉ���ÓI�t�@�C���Ƃ���
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Files")),
                RequestPath = new PathString("/Files")
            });
            app.UseRequestLocalizationByCulture();          // �J���`���֌W�����ǂ悭������Ȃ�
            app.UseRouting();                               // ���[�e�B���O
            app.UseAuthentication();                        // ���[�U�[���Z�L�����e�B�ŕی삳�ꂽ���\�[�X�ɃA�N�Z�X����O�ɁA���[�U�[�̔F�؂����s����܂��B
            app.UseAuthorization();                         // ���[�U�[���Z�L�����e�B�ŕی삳�ꂽ���\�[�X�ɃA�N�Z�X���邱�Ƃ����F����܂��B
            app.UseHangfireDashboard("/jobs", new DashboardOptions              // Hangfire�i�莞���s�����j�̃_�b�V���{�[�h
            {
                DashboardTitle = localizer["BlazorHero Jobs"],
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });
            app.UseEndpoints();
            app.ConfigureSwagger();                         // Swagger�̐ݒ�
            app.Initialize(_configuration);                 // IDatabaseSeeder�ŏ������iDB�����f�[�^���܂��Ȃ��ꍇ�Ƀf�[�^������j
        }
    }
}