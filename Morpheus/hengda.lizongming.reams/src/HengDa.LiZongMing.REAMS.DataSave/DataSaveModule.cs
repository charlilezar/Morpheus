//using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Caching;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Account;
using Volo.Abp.Identity;

using HengDa.LiZongMing.REAMS.EntityFrameworkCore;
using HengDa.LiZongMing.REAMS.CtrlServices;

namespace HengDa.LiZongMing.REAMS.DataSave
{

    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(REAMSDomainModule),
        typeof(REAMSEntityFrameworkCoreModule),
        typeof(REAMSApplicationModule),
        typeof(REAMSApplicationContractsModule),
        typeof(AbpAccountApplicationModule),
        typeof(AbpEntityFrameworkCoreModule),
        typeof(AbpIdentityApplicationModule)
    )]
    public class DataSaveModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var hostEnvironment = context.Services.GetSingletonInstance<IHostEnvironment>();
            context.Services.AddHostedService<DataSaveHostedService>();
             ////context.Services.AddTransient<IIotFactory, IotFactory>();
            //////context.Services.AddSingleton<IotFactory>();

            ////context.Services.AddAssembly(this.GetType().Assembly);


            //api需要的数据库连接
            Configure<AbpDbContextOptions>(options =>
            {
                //options.UseSqlServer();

                //换数据库mysql
                options.UseMySQL();
            });

            // 后台作业  //不需要其它的后台任务

            Configure<AbpBackgroundJobOptions>(options =>
            {
                options.IsJobExecutionEnabled = false; //禁用后台作业执行

            });
            Configure<AbpBackgroundJobWorkerOptions>(options =>
            {
                options.JobPollPeriod = 86400; //1 days (as seconds) //后台作业执行间隔
            });

            //ConfigureLocalization();
            //ConfigureCache(configuration);
            ////ConfigureVirtualFileSystem(context);
            ////ConfigureRedis(context, configuration, hostingEnvironment);
            ////ConfigureCors(context, configuration);
            ////ConfigureSwaggerServices(context, configuration);
            ////审计日志的配置
            //Configure<AbpAuditingOptions>(options =>
            //{
            //    options.IsEnabled = false; //Disables the auditing system
            //});
        }
        private void ConfigureCache(IConfiguration configuration)
        {
            Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "REAMS:"; });
        }

        //private void ConfigureLocalization()
        //{
        //    Configure<AbpLocalizationOptions>(options =>
        //    {
        //        //options.Languages.Add(new LanguageInfo("ar", "ar", "العربية"));
        //        //options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština"));
        //        //options.Languages.Add(new LanguageInfo("en", "en", "English"));
        //        //options.Languages.Add(new LanguageInfo("en-GB", "en-GB", "English (UK)"));
        //        //options.Languages.Add(new LanguageInfo("fr", "fr", "Français"));
        //        //options.Languages.Add(new LanguageInfo("hu", "hu", "Magyar"));
        //        //options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
        //        //options.Languages.Add(new LanguageInfo("ru", "ru", "Русский"));
        //        //options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
        //        options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
        //        //options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
        //        //options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "Deutsch", "de"));
        //        //options.Languages.Add(new LanguageInfo("es", "es", "Español", "es"));
        //    });
        //}

        //private void ConfigureRedis(
        //    ServiceConfigurationContext context,
        //    IConfiguration configuration,
        //    IWebHostEnvironment hostingEnvironment)
        //{
        //    if (!hostingEnvironment.IsDevelopment())
        //    {
        //        var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
        //        context.Services
        //            .AddDataProtection()
        //            .PersistKeysToStackExchangeRedis(redis, "REAMS-Protection-Keys");
        //    }
        //}

       

    }
}
