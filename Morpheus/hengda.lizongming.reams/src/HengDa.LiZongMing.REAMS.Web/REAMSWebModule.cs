using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HengDa.LiZongMing.REAMS.EntityFrameworkCore;
using HengDa.LiZongMing.REAMS.Localization;
using HengDa.LiZongMing.REAMS.MultiTenancy;
using HengDa.LiZongMing.REAMS.Web.Menus;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity.Web;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.Web;
using Volo.Abp.SettingManagement.Web;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement.Web;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.UI;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.BackgroundJobs;
using System.Linq;
using Volo.Abp.Auditing;
using Microsoft.AspNetCore.Cors;
using System.Collections.Generic;
using Microsoft.IdentityModel.Logging;
using Microsoft.AspNetCore.HttpOverrides;

namespace HengDa.LiZongMing.REAMS.Web
{
    [DependsOn(
        typeof(REAMSHttpApiModule),
        typeof(REAMSApplicationModule),
        typeof(REAMSEntityFrameworkCoreModule),
        typeof(AbpAutofacModule),
        typeof(AbpIdentityWebModule),
        typeof(AbpSettingManagementWebModule),
        typeof(AbpAccountWebIdentityServerModule),
        typeof(AbpAspNetCoreMvcUiBasicThemeModule),
        typeof(AbpAspNetCoreAuthenticationJwtBearerModule),
        typeof(AbpTenantManagementWebModule),
        typeof(AbpAspNetCoreSerilogModule),
        typeof(AbpSwashbuckleModule)
        )]
    public class REAMSWebModule : AbpModule
    {
        private const string DefaultCorsPolicyName = "Default";
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
            {
                options.AddAssemblyResource(
                    typeof(REAMSResource),
                    typeof(REAMSDomainModule).Assembly,
                    typeof(REAMSDomainSharedModule).Assembly,
                    typeof(REAMSApplicationModule).Assembly,
                    typeof(REAMSApplicationContractsModule).Assembly,
                    typeof(REAMSWebModule).Assembly
                );
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            var configuration = context.Services.GetConfiguration();
            // 后台作业

            Configure<AbpBackgroundJobOptions>(options =>
            {
                options.IsJobExecutionEnabled = false; //禁用后台作业执行

            });
            Configure<AbpBackgroundJobWorkerOptions>(options =>
            {
                options.JobPollPeriod = 86400; //1 days (as seconds) //后台作业执行间隔
            });

            //增加后台作业
            //Configure<AbpBackgroundJobOptions>(options =>
            //{
            //    options.AddJob<BackgroundTcpGetTestAtmosphereJob>();
            //});

            //需要去调用添加一个新作业才会调起的

            //ConfigureMongoDbContext(context, configuration);

            ConfigureUrls(configuration);
            ConfigureBundles();
            ConfigureAuthentication(context, configuration);
            ConfigureAutoMapper();
            ConfigureVirtualFileSystem(hostingEnvironment);
            ConfigureLocalizationServices();
            ConfigureNavigationServices();
            ConfigureAutoApiControllers();
            ConfigureSwaggerServices(context.Services, configuration);

            context.Services.AddCors(options =>
            {

                options.AddPolicy(DefaultCorsPolicyName, builder =>
                {

                    builder
                        .WithOrigins(   //列表指定来源网址
                            (configuration["App:CorsOrigins"] + "").Replace(";", ",")
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )//.WithOrigins("http://localhost:9527")
                         //.AllowAnyOrigin()  //新版本增强了安全不准用 AllowAnyOrigin()+ AllowAnyMethod()
                         //.SetIsOriginAllowed(_ => true)  //变通替代 以前的 AllowAnyOrigin()
                        .WithAbpExposedHeaders()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            //审计日志的配置
            Configure<AbpAuditingOptions>(options =>
            {
                options.IsEnabled = false; //Disables the auditing system
            });

        }
        private void ConfigureMongoDbContext(ServiceConfigurationContext context, IConfiguration configuration)
        {

            //context.Services.AddMongoDbContext<YunMongoDbContext>(options =>
            //{
            //    options.AddDefaultRepositories(includeAllEntities: true);
            //});

            ////模板默认在 .MongoDB 项目中禁用了工作单元事务. 如果你的MongoDB服务器支持事务,你可以手动启用工作单元的事务:
            ////Configure<AbpUnitOfWorkDefaultOptions>(options =>
            ////{
            ////    options.TransactionBehavior = UnitOfWorkTransactionBehavior.Enabled;
            ////});
        }
        private void ConfigureUrls(IConfiguration configuration)
        {
            Configure<AppUrlOptions>(options =>
            {
                options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            });
        }

        private void ConfigureBundles()
        {
            Configure<AbpBundlingOptions>(options =>
            {
                options.StyleBundles.Configure(
                    BasicThemeBundles.Styles.Global,
                    bundle =>
                    {
                        bundle.AddFiles("/global-styles.css");
                    }
                );
            });
        }

        private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAuthentication()
            #region 4.x版用的,vue的登陆不好用
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["AuthServer:Authority"];
                    //options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                    options.RequireHttpsMetadata = false;
                    options.Audience = "REAMS";
                    //我再加
                    options.IncludeErrorDetails = true;
                    //options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    //{
                    //    ValidateIssuer = true,
                    //    ValidIssuer = "Security:Tokens:Issuer",
                    //    ValidateAudience = true,
                    //    ValidAudience = "Security:Tokens:Audience",
                    //    ValidateIssuerSigningKey = true,
                    //    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes("Security:Tokens:Key"))
                    //};

                })
            #endregion
            #region 3.x用的,验证jwt时需要
                 //.AddIdentityServerAuthentication(options =>
                 //{
                 //    options.Authority = configuration["AuthServer:Authority"];
                 //    options.RequireHttpsMetadata = false;
                 //    options.ApiName = "Yun";
                 //    options.JwtBackChannelHandler = new HttpClientHandler()
                 //    {
                 //        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                 //    };
                 //})
            #endregion
                 ;
        }

        private void ConfigureAutoMapper()
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<REAMSWebModule>();
            });
        }

        private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
        {
            if (hostingEnvironment.IsDevelopment())
            {
                Configure<AbpVirtualFileSystemOptions>(options =>
                {
                    options.FileSets.ReplaceEmbeddedByPhysical<REAMSDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}HengDa.LiZongMing.REAMS.Domain.Shared"));
                    options.FileSets.ReplaceEmbeddedByPhysical<REAMSDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}HengDa.LiZongMing.REAMS.Domain"));
                    options.FileSets.ReplaceEmbeddedByPhysical<REAMSApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}HengDa.LiZongMing.REAMS.Application.Contracts"));
                    options.FileSets.ReplaceEmbeddedByPhysical<REAMSApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}HengDa.LiZongMing.REAMS.Application"));
                    options.FileSets.ReplaceEmbeddedByPhysical<REAMSWebModule>(hostingEnvironment.ContentRootPath);
                });
            }
        }

        private void ConfigureLocalizationServices()
        {
            Configure<AbpLocalizationOptions>(options =>
            {

                //options.Languages.Add(new LanguageInfo("ar", "ar", "العربية"));
                //options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština"));
                options.Languages.Add(new LanguageInfo("en", "en", "English"));
                //options.Languages.Add(new LanguageInfo("hu", "hu", "Magyar"));
                //options.Languages.Add(new LanguageInfo("fr", "fr", "Français"));
                //options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
                //options.Languages.Add(new LanguageInfo("ru", "ru", "Русский"));
                //options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
                options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
                //options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
                //options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "Deutsch", "de"));
                //options.Languages.Add(new LanguageInfo("es", "es", "Español"));
            });
        }

        private void ConfigureNavigationServices()
        {
            Configure<AbpNavigationOptions>(options =>
            {
                options.MenuContributors.Add(new REAMSMenuContributor());
            });
        }

        private void ConfigureAutoApiControllers()
        {
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(REAMSApplicationModule).Assembly, opts =>
                {
                    //可以自己控制api路径的转换方法
                    //opts.UrlActionNameNormalizer
                });
            });
        }

        private void ConfigureSwaggerServices(IServiceCollection services, IConfiguration configuration)
        {
            string _oAuthAuthority = configuration["AuthServer:Authority"];
            services.AddAbpSwaggerGenWithOAuth(
                 _oAuthAuthority,
                 new Dictionary<string, string>
                 {
                    {"REAMS", "REAMS API"}
                 },
                 options =>
                 {
                     options.SwaggerDoc("v1", new OpenApiInfo { Title = "云平台API", Version = "v1" });
                     options.DocInclusionPredicate((docName, description) => true);
                     options.CustomSchemaIds(type => type.FullName);
                     //多个防冲突
                     //options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                     // 解析xml里的注释到api界面
                     var xmlapppath = Path.Combine(AppContext.BaseDirectory, "HengDa.LiZongMing.REAMS.Application.Contracts.xml");
                     if (File.Exists(xmlapppath))
                     {
                         options.IncludeXmlComments(xmlapppath);
                     }
                     //var xmlapipath = Path.Combine(AppContext.BaseDirectory, "Aming.IotManage.HttpApi.xml");
                     //if (File.Exists(xmlapipath))
                     //{
                     //    options.IncludeXmlComments(xmlapipath);
                     //}
                 }

             );

        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            IdentityModelEventSource.ShowPII = true; //显示详细错误

            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            //放在反向代理后面时，修复一下原始IP等信息  https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-3.1
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAbpRequestLocalization();

            if (!env.IsDevelopment())
            {
                //mvc显示自定义错误页
                //app.UseErrorPage();
            }

            app.UseCorrelationId();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors(DefaultCorsPolicyName);
            app.UseAuthentication();
            app.UseJwtTokenMiddleware();

            if (MultiTenancyConsts.IsEnabled)
            {
                app.UseMultiTenancy();
            }

            app.UseUnitOfWork();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseAbpSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "REAMS API v 1.0");
                var configuration = context.GetConfiguration();
                options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
                options.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]);
            });
            app.UseAuditing();
            app.UseAbpSerilogEnrichers();
            app.UseConfiguredEndpoints();
        }
    }
}
