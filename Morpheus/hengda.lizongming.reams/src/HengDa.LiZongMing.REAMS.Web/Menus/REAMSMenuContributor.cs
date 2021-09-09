using System.Threading.Tasks;
using HengDa.LiZongMing.REAMS.Localization;
using HengDa.LiZongMing.REAMS.MultiTenancy;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;

namespace HengDa.LiZongMing.REAMS.Web.Menus
{
    public class REAMSMenuContributor : IMenuContributor
    {
        public async Task ConfigureMenuAsync(MenuConfigurationContext context)
        {
            if (context.Menu.Name == StandardMenus.Main)
            {
                await ConfigureMainMenuAsync(context);
            }
        }

#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        private async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        {
            var administration = context.Menu.GetAdministration();
            var l = context.GetLocalizer<REAMSResource>();

            context.Menu.Items.Insert(
                0,
                new ApplicationMenuItem(
                    REAMSMenus.Home,
                    l["Menu:Home"],
                    "~/",
                    icon: "fas fa-home",
                    order: 0
                )
            );
            #pragma warning disable CS0162 // 检测到无法访问的代码
            if (MultiTenancyConsts.IsEnabled)
            {
                administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
            }
            else
            {
                administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
            }

            administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
            administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);
#pragma warning restore CS0162 // 检测到无法访问的代码




            //swagger 入口
            context.Menu.AddItem(new ApplicationMenuItem(
                        "API",
                        l["API"],
                        "~/swagger/index.html",
                        icon: "fa fa-cog"
                    ));
        }
    }
}
