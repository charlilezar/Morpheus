using System;
using System.Text;
using System.Threading.Tasks;
using HengDa.LiZongMing.REAMS.Devices.Dtos;
using HengDa.LiZongMing.REAMS.Room.Dtos;
using HengDa.LiZongMing.REAMS.ZCQ;
using HengDa.LiZongMing.REAMS.ZCQ.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace HengDa.LiZongMing.REAMS.DataSave
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            try
            {
                //日志改用配置文件
                var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                     .AddJsonFile("log.json")
                     .Build();
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();
                Log.Information("Starting DataSave console host.");
                await CreateHostBuilder(args).RunConsoleAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "DataSave Host terminated unexpectedly!");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseAutofac()
                .UseSerilog()
                .ConfigureAppConfiguration((context, config) =>
                {
                    //setup your additional configuration sources
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddApplication<DataSaveModule>();
                    services.AddSingleton<Aming.DTU.Config.IDataEndPoint, Aming.DTU.Config.MqttClientConfig>();
                    services.AddSingleton<Aming.DTU.Config.IDataEndPoint, Aming.DTU.Config.TcpClientConfig>();
                    services.AddSingleton<CtrlServices.CtrlHDZCQService>();
                    services.AddSingleton<CtrlServices.CtrlHVEService>();
                    services.AddSingleton<CtrlServices.HDZCQDataService>();
                    services.AddSingleton<CtrlServices.HVEDataService>();
                   // services.AddSingleton<CtrlServices.CtrlHDNaISService>();
                   // services.AddSingleton<CtrlServices.HDNaISDataService>();
                    services.AddTransient<ZcqRunStatus>();
                    services.AddTransient<ZcqRunStatusDto>();
                    services.AddTransient<Wpf.Dto.SSRealDataDto>();
                    services.AddTransient<DeviceDto>();
                   // services.AddTransient<NaI.Dtos.NaiRecordDto>();
                    services.AddTransient<RoomRunStatusDto>();
                    services.AddTransient<Aming.DTU.Config.UniMessageBase>();
                    services.AddSingleton<CtrlServices.CtrlForwardMqttServer>();
                    services.AddSingleton<EchoServer>(op=>new EchoServer("0.0.0.0",51001));
                 


                    //services.AddHostedService<DataSaveHostedService>();
                    services.AddSingleton<Aming.DTU.Config.IDataEndPoint, Aming.DTU.Config.TcpClientConfig>();
                    services.AddSingleton<CtrlServices.CtrlHDSSService>();
                    services.AddTransient<Aming.DTU.Config.UniMessageBase>();
                });
    }
}
