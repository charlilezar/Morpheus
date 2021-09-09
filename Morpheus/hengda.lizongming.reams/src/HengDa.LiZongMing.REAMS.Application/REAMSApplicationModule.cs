using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace HengDa.LiZongMing.REAMS
{
    [DependsOn(
        typeof(REAMSDomainModule),
        typeof(AbpAccountApplicationModule),
        typeof(REAMSApplicationContractsModule),
        typeof(AbpIdentityApplicationModule),
        typeof(AbpPermissionManagementApplicationModule),
        typeof(AbpTenantManagementApplicationModule),
        typeof(AbpFeatureManagementApplicationModule),
        typeof(AbpSettingManagementApplicationModule)
        )]
    public class REAMSApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<REAMSApplicationModule>();
            });
            //别忘了加这个Mapper
            context.Services.AddAutoMapperObjectMapper<REAMSApplicationModule>();
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddProfile<REAMSApplicationAutoMapperProfile>(validate: true);
            });
        }
    }
}
