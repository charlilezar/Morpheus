using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volo.Abp;

namespace HengDa.LiZongMing.REAMS.DataSave
{
    public class DataSaveHostedService : IHostedService
    {
        private readonly IAbpApplicationWithExternalServiceProvider _application;
        private readonly IServiceProvider _serviceProvider;
        public static IotDeviceCtrlServiceFactory _helloWorldService;
        private ILoggerFactory loggerFactory;
        public ILogger Logger { get; set; }

        public DataSaveHostedService(
            IAbpApplicationWithExternalServiceProvider application,
            IServiceProvider serviceProvider,
            IotDeviceCtrlServiceFactory helloWorldService,
            ILoggerFactory loggerFactory)
        {
            _application = application;
            _serviceProvider = serviceProvider;
            _helloWorldService = helloWorldService;

            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<DataSaveHostedService>();

            //重点，
            Aming.Tools.IocHelper.ServiceProvider = _serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _application.Initialize(_serviceProvider);

            Logger.LogInformation("启动了！！");

           await  _helloWorldService.StartAsync();



            /*
                        //调用后台保存任务
                        AsyncHelper.RunSync(async () =>
                       {
                           using (var scope = _serviceProvider.CreateScope())
                           {
                                var uowManager = scope.ServiceProvider.GetRequiredService<Volo.Abp.Uow.IUnitOfWorkManager>();

                               using (var uow = uowManager.Begin())
                               {
                                   try
                                   {
                                       var iotFactory = new IotFactory(loggerFactory, _serviceScopeFactory, _serviceProvider);
                                       ////_helloWorldService.SayHello();
                                       await iotFactory.CreateIotDeviceConnect();
                                   }
                                   catch (Exception ex)
                                   {

                                       Logger.LogException(ex);
                                   }
                               }
                           }
                       });
            */
            return;// Task.CompletedTask;

            //using (var application = AbpApplicationFactory.Create<DataSaveModule>(options =>
            //            {
            //                options.UseAutofac();
            //                options.Services.AddLogging(c => c.AddSerilog());
            //            }))
            //{
            //    application.Initialize();

            //    Logger.LogInformation("启动了！！");
            //    //await application
            //    //    .ServiceProvider
            //    //    .GetRequiredService<REAMSDbMigrationService>()
            //    //    .MigrateAsync();

            //    //调用后台保存任务
            //    AsyncHelper.RunSync(async () =>
            //   {
            //       using (var scope = application.ServiceProvider.CreateScope())
            //       {
            //           try
            //           {
            //               //var iotFactory = new IotFactory(loggerFactory, _serviceScopeFactory, _serviceProvider);
            //               var iotFactory = application.ServiceProvider.GetService<IotFactory>();
            //               ////_helloWorldService.SayHello();
            //               await iotFactory.CreateIotDeviceConnect();
            //           }
            //           catch (Exception ex)
            //           {

            //               Logger.LogException(ex);
            //           }
            //       }
            //   });


            //    //application.Shutdown();

            //    //_hostApplicationLifetime.StopApplication();


            //}

        }



        public Task StopAsync(CancellationToken cancellationToken)
        {
            _application.Shutdown();

            return Task.CompletedTask;
        }
    }
}
