using System.Threading.Tasks;

namespace HengDa.LiZongMing.REAMS.Data
{
    public interface IREAMSDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
