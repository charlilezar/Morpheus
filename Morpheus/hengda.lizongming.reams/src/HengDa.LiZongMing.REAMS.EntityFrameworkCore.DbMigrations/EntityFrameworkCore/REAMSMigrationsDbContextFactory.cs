using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HengDa.LiZongMing.REAMS.EntityFrameworkCore
{
    /* This class is needed for EF Core console commands
     * (like Add-Migration and Update-Database commands) */
    public class REAMSMigrationsDbContextFactory : IDesignTimeDbContextFactory<REAMSMigrationsDbContext>
    {
        public REAMSMigrationsDbContext CreateDbContext(string[] args)
        {
            REAMSEfCoreEntityExtensionMappings.Configure();

            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<REAMSMigrationsDbContext>()
                //.UseSqlServer(configuration.GetConnectionString("Default"));
                .UseMySql(configuration.GetConnectionString("Default"),ServerVersion.AutoDetect(configuration.GetConnectionString("Default")));

            return new REAMSMigrationsDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../HengDa.LiZongMing.REAMS.DbMigrator/"))
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
