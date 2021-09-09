using HengDa.LiZongMing.REAMS.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace HengDa.LiZongMing.REAMS.Web.Pages
{
    /* Inherit your PageModel classes from this class.
     */
    public abstract class REAMSPageModel : AbpPageModel
    {
        protected REAMSPageModel()
        {
            LocalizationResourceType = typeof(REAMSResource);
        }
    }
}