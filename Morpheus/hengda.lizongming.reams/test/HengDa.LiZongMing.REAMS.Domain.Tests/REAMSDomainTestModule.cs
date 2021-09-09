using HengDa.LiZongMing.REAMS.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace HengDa.LiZongMing.REAMS
{
    [DependsOn(
        typeof(REAMSEntityFrameworkCoreTestModule)
        )]
    public class REAMSDomainTestModule : AbpModule
    {

    }
}