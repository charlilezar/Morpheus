using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;

namespace HengDa.LiZongMing.REAMS.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<REAMSWebModule>();
        }

        public void Configure(IApplicationBuilder app)
        {
            //显示所有错误
            IdentityModelEventSource.ShowPII = true;
            
            app.InitializeApplication();
        }
    }
}
