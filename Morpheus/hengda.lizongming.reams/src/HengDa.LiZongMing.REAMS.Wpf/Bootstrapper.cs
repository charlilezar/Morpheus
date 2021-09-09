using Autofac;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
//using System.Windows;

using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Volo.Abp;
using HengDa.LiZongMing.REAMS.Wpf.UC;
using Microsoft.Extensions.Configuration;
using System.Text;
using HengDa.LiZongMing.REAMS.CtrlServices;
using HengDa.LiZongMing.REAMS.DataSave.ZCQ;
using HengDa.LiZongMing.REAMS.ZCQ;


namespace HengDa.LiZongMing.REAMS
{

    /// <summary>
    /// 
    /// 参考 https://github.com/canton7/Stylet/blob/master/Bootstrappers/AutofacBootstrapper.cs
    /// </summary>
    /// <typeparam name="TRootViewModel"></typeparam>
    class AbpBootstrapper : BootstrapperBase //where TRootViewModel : class
    {
        #region MyRegion
        //private IContainer container;

        //private TRootViewModel _rootViewModel;
        //protected virtual TRootViewModel RootViewModel
        //{
        //    get {
        //        return this._rootViewModel ?? (this._rootViewModel = (TRootViewModel)this.GetInstance(typeof(TRootViewModel)));
        //    }
        //}
        #endregion

        #region MyRegion
        private readonly IHost _host;
        private readonly IAbpApplicationWithExternalServiceProvider _application;

        #endregion
        public AbpBootstrapper()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            _host = Host
            .CreateDefaultBuilder(null)
            .UseAutofac()
            .UseSerilog()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddApplication<REAMSModule>();

                //加上Sytled
                RegisterType(services);

            }).Build();
            _application = _host.Services.GetService<IAbpApplicationWithExternalServiceProvider>();
        }
        protected override void ConfigureBootstrapper()
        {
            #region 原来的这种不需要了
            //var builder = new ContainerBuilder();
            //this.DefaultConfigureIoC(builder);
            //this.ConfigureIoC(builder);
            //this.container = builder.Build();

            #endregion



        }

        /// <summary>
        /// Carries out default configuration of the IoC container. Override if you don't want to do this
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        protected virtual void RegisterType(IServiceCollection services)
        {
            var viewManagerConfig = new ViewManagerConfig()
            {
                ViewFactory = this.GetInstance,
                ViewAssemblies = new List<Assembly>() { this.GetType().Assembly }
            };
            services.AddSingleton(typeof(IViewManager),new ViewManager(viewManagerConfig));
            services.AddSingleton(typeof(IWindowManagerConfig),this);
            services.AddTransient<IWindowManager, WindowManager>();
            services.AddTransient<IEventAggregator, EventAggregator>();
            services.AddTransient<IMessageBoxViewModel, MessageBoxViewModel>();
            services.AddTransient<MessageBoxView>();

            //services.AddSingleton<MainWindow>();
            services.AddSingleton<RootViewModel>();
            services.AddSingleton<RootView>();
            //气碘采样
            services.AddSingleton<UcHDZCQViewModel>();
            services.AddSingleton<UcHDZCQView>(); 
            //干湿沉降
            services.AddSingleton<UcHDZJCViewModel>();
            services.AddSingleton<UcHDZJCView>();
            //感雨器
            services.AddSingleton<UcHDRNSViewModel>();
            services.AddSingleton<UcHDRNSView>();
            //气溶胶
            services.AddSingleton<UcHDNBSViewModel>();
            services.AddSingleton<UcHDNBSView>();


            services.AddSingleton<Aming.DTU.Config.IDataEndPoint, Aming.DTU.Config.MqttClientConfig>();
            services.AddSingleton<CtrlServices.CtrlHDZCQService>();
            services.AddSingleton<CtrlServices.CtrlHDSSService>();

            services.AddTransient<ZcqRunStatus>();
           // services.AddTransient<SSDataSaveService>();
            services.AddSingleton<HDZCQDataService>();
            services.AddAssembly(this.GetType().Assembly);

            //builder.RegisterInstance<IViewManager>(new ViewManager(viewManagerConfig));
            //builder.RegisterType<MessageBoxView>();

            //builder.RegisterInstance<IWindowManagerConfig>(this).ExternallyOwned();
            //builder.RegisterType<WindowManager>().As<IWindowManager>().SingleInstance();
            //builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            //builder.RegisterType<MessageBoxViewModel>().As<IMessageBoxViewModel>().ExternallyOwned(); // Not singleton!

            //// See https://github.com/canton7/Stylet/discussions/211
            //builder.RegisterAssemblyTypes(this.GetType().Assembly).Where(x => !x.Name.Contains("ProcessedByFody")).ExternallyOwned();
        }


        public override object GetInstance(Type type)
        {
            //return this.container.Resolve(type);
            var obj= _host.Services.GetService(type);
            return obj;
        }

        protected async override void OnStart(/*StartupEventArgs e*/)
        {
//            Log.Logger = new LoggerConfiguration()
//#if DEBUG
//                .MinimumLevel.Debug()
//#else
//                .MinimumLevel.Information()
//#endif
//                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
//                .Enrich.FromLogContext()
//                .WriteTo.Async(c => c.File("Logs/logs.txt"))
//                .CreateLogger();

//            try
//            {
//                Log.Information("Starting WPF host.");
//                await _host.StartAsync();
//                _application.Initialize(_host.Services);
//                //abp用到的入口页
//                //_host.Services.GetService<MainWindow>()?.Show();

//            }
//            catch (Exception ex)
//            {
//                Log.Fatal(ex, "Host terminated unexpectedly!");
//            }
//            finally
//            {
//                Log.CloseAndFlush();
//            }

            try
            {
                //日志改用配置文件
                var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                     .AddJsonFile("log.json")
                     .Build();
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();

                Log.Information("Starting WPF host.");
                await _host.StartAsync();
                _application.Initialize(_host.Services);
                //abp用到的入口页
                //_host.Services.GetService<MainWindow>()?.Show();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");

            }
            finally
            {
                //Log.CloseAndFlush();
            }
        }

        protected override void Launch()
        {
            //base.DisplayRootView(this.RootViewModel);
            var RootViewModel = _host.Services.GetService<RootViewModel>();
            base.DisplayRootView(RootViewModel);
        }
        protected async override void OnExit(ExitEventArgs e)
        {
            _application.Shutdown();
            await _host.StopAsync();
            _host.Dispose();
        }
        public override void Dispose()
        {
            //ScreenExtensions.TryDispose(this._rootViewModel);
            //if (this.container != null)
            //    this.container.Dispose();

            base.Dispose();
        }
    }
}