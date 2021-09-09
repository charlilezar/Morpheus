using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace HengDa.LiZongMing.REAMS.EntityFrameworkCore
{
    [DependsOn(
        typeof(REAMSEntityFrameworkCoreModule)
        )]
    public class REAMSEntityFrameworkCoreDbMigrationsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<REAMSMigrationsDbContext>();
        }
    }
}
