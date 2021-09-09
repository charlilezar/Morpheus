using Volo.Abp.Modularity;

namespace HengDa.LiZongMing.REAMS
{
    [DependsOn(
        typeof(REAMSApplicationModule),
        typeof(REAMSDomainTestModule)
        )]
    public class REAMSApplicationTestModule : AbpModule
    {

    }
}