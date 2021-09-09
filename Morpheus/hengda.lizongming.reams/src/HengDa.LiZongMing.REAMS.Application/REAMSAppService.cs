using System;
using System.Collections.Generic;
using System.Text;
using HengDa.LiZongMing.REAMS.Localization;
using Volo.Abp.Application.Services;

namespace HengDa.LiZongMing.REAMS
{
    /* Inherit your application services from this class.
     */
    public abstract class REAMSAppService : ApplicationService
    {
        protected REAMSAppService()
        {
            LocalizationResource = typeof(REAMSResource);
        }
    }
}
