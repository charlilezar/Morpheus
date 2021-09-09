using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace HengDa.LiZongMing.REAMS.Web
{
    [Dependency(ReplaceServices = true)]
    public class REAMSBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "REAMS";
    }
}
