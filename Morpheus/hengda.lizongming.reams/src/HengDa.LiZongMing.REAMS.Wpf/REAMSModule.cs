using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace HengDa.LiZongMing.REAMS
{
    [DependsOn(typeof(AbpAutofacModule))]
    public class REAMSModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            //context.Services.AddSingleton<MainWindow>();
            //context.Services.AddSingleton<RootViewModel>();
            //context.Services.AddSingleton<RootView>();

        }
    }
}
