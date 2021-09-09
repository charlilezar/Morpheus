using HengDa.LiZongMing.REAMS.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace HengDa.LiZongMing.REAMS.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(REAMSEntityFrameworkCoreDbMigrationsModule),
        typeof(REAMSApplicationContractsModule)
        )]
    public class REAMSDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
            //api需要的数据库连接
            Configure<AbpDbContextOptions>(options =>
            {
                //options.UseSqlServer();
                //换数据库mysql
                options.UseMySQL();
            });
        }
    }
}
