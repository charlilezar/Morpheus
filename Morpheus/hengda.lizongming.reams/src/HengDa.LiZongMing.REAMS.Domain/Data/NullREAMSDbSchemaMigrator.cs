using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace HengDa.LiZongMing.REAMS.Data
{
    /* This is used if database provider does't define
     * IREAMSDbSchemaMigrator implementation.
     */
    public class NullREAMSDbSchemaMigrator : IREAMSDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}