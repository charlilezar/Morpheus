using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HengDa.LiZongMing.REAMS.Data;
using Volo.Abp.DependencyInjection;

namespace HengDa.LiZongMing.REAMS.EntityFrameworkCore
{
    public class EntityFrameworkCoreREAMSDbSchemaMigrator
        : IREAMSDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreREAMSDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the REAMSMigrationsDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<REAMSMigrationsDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}