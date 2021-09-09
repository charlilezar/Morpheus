using HengDa.LiZongMing.REAMS.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace HengDa.LiZongMing.REAMS.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class REAMSController : AbpController
    {
        protected REAMSController()
        {
            LocalizationResource = typeof(REAMSResource);
        }
    }
}