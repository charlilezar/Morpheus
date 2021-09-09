using System;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace HengDa.LiZongMing.REAMS
{
    public static class TaskHelper
    {
        /// <summary>
        /// 在新的Uow和指定TenantId下运行
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static void RunByUowTenantId(Guid? TenantId,Func<Task> action)
        {
            var _serviceProvider = Aming.Tools.IocHelper.ServiceProvider;
            using (var scope = _serviceProvider.CreateScope())
            {
                var uowManager = scope.ServiceProvider.GetRequiredService<Volo.Abp.Uow.IUnitOfWorkManager>();

                using (var uow = uowManager.Begin())
                {
                    ICurrentTenant currentTenant = _serviceProvider.GetService<ICurrentTenant>();
                    using (currentTenant.Change(TenantId))
                    {
                        AsyncContext.Run(action);
                    }
                    
                }
            }
            
        }
        //public static TResult RunUowTenantId<TResult>(Func<Task<TResult>> func)
        //{
        //    return AsyncContext.Run(func);
        //}
    }
}
