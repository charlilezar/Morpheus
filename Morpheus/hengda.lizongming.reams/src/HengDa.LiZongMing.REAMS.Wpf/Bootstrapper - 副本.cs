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

namespace HengDa.LiZongMing.REAMS
{
    class AbpBootstrapper : AbpBootstrapperBase<RootViewModel>
    {
        public AbpBootstrapper()
        {

        }
    }
    /// <summary>
    /// 
    /// 参考 https://github.com/canton7/Stylet/blob/master/Bootstrappers/AutofacBootstrapper.cs
    /// </summary>
    /// <typeparam name="TRootViewModel"></typeparam>
    class AbpBootstrapperBase<TRootViewModel> : BootstrapperBase where TRootViewModel : class
    {
        #region MyRegion
        //private IContainer container;

        private TRootViewModel _rootViewModel;
        protected virtual TRootViewModel RootViewModel
        {
            get {
                return this._rootViewModel ?? (this._rootViewModel = (TRootViewModel)this.GetInstance(typeof(TRootViewModel)));
            }
        }
        #endregion

        #region MyRegion
        private readonly IHost _host;
        private readonly IAbpApplicationWithExternalServiceProvider _application;

        #endregion
        public AbpBootstrapperBase()
        {


            

            _host = Host
            .CreateDefaultBuilder(null)
            .UseAutofac()
            .UseSerilog()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddApplication<REAMSModule>();

                #region MyRegion
                //加上Sytled
                var builder = new ContainerBuilder();
                this.DefaultConfigureIoC(builder);
                this.ConfigureIoC(builder);
                services.AddAutofacServiceProviderFactory(builder); 
                #endregion

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
        /// <summary>
        /// Carries out default configuration of the IoC container. Override if you don't want to do this
        /// </summary>
        protected virtual void DefaultConfigureIoC(ContainerBuilder builder)
        {
            var viewManagerConfig = new ViewManagerConfig()
            {
                ViewFactory = this.GetInstance,
                ViewAssemblies = new List<Assembly>() { this.GetType().Assembly }
            };
            builder.RegisterInstance<IViewManager>(new ViewManager(viewManagerConfig));
            builder.RegisterType<MessageBoxView>();

            builder.RegisterInstance<IWindowManagerConfig>(this).ExternallyOwned();
            builder.RegisterType<WindowManager>().As<IWindowManager>().SingleInstance();
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            builder.RegisterType<MessageBoxViewModel>().As<IMessageBoxViewModel>().ExternallyOwned(); // Not singleton!

            // See https://github.com/canton7/Stylet/discussions/211
            builder.RegisterAssemblyTypes(this.GetType().Assembly).Where(x => !x.Name.Contains("ProcessedByFody")).ExternallyOwned();
        }

        /// <summary>
        /// Override to add your own types to the IoC container.
        /// </summary>
        protected virtual void ConfigureIoC(ContainerBuilder builder) { }

        public override object GetInstance(Type type)
        {
            //return this.container.Resolve(type);
            return _host.Services.GetService(type);
        }

        protected async override void OnStart(/*StartupEventArgs e*/)
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Async(c => c.File("Logs/logs.txt"))
                .CreateLogger();

            try
            {
                Log.Information("Starting WPF host.");
                await _host.StartAsync();
                //App的Initialize(_host.Services);

                //abp用到的入口页
                //_host.Services.GetService<MainWindow>()?.Show();

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        protected override void Launch()
        {
            base.DisplayRootView(this.RootViewModel);
        }
        protected async override void OnExit(ExitEventArgs e)
        {
            _application.Shutdown();
            await _host.StopAsync();
            _host.Dispose();
        }
        public override void Dispose()
        {
            ScreenExtensions.TryDispose(this._rootViewModel);
            //if (this.container != null)
            //    this.container.Dispose();

            base.Dispose();
        }
    }
}