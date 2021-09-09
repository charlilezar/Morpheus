using HengDa.LiZongMing.REAMS.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace HengDa.LiZongMing.REAMS.Permissions
{
    public class REAMSPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(REAMSPermissions.GroupName);

            //Define your own permissions here. Example:
            //myGroup.AddPermission(REAMSPermissions.MyPermission1, L("Permission:MyPermission1"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<REAMSResource>(name);
        }
    }
}
