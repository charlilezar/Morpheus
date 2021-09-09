using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.IdentityServer.ApiResources;
using Volo.Abp.IdentityServer.ApiScopes;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.IdentityServer.IdentityResources;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;


namespace HengDa.LiZongMing.REAMS.IdentityServer
{
    public class NewTenantDataSeedContributor
    {

        /// <summary>
        /// ���Ĭ���⻧��Ҫ�� XXX DbMigrationService.MigrateAsync����λ�õ���
        /// </summary>
        public static  async Task SeedTenantAsync( ITenantRepository _tenantRepository)
        {
           
            string[] TenantNames = new string[] { "�ڲ�", "��˾" };
            string[] TenantIds = new string[] { "39f99bf5-c4eb-33d2-a623-591eec290001", "39f99bf5-c4eb-33d2-a623-591eec290002" };

            for (int i0 = 0; i0 < TenantNames.Length; i0++)
            {
                if (await _tenantRepository.FindAsync(Guid.Parse(TenantIds[i0])) == null)
                {
                    var tenant = new MyTenant(Guid.Parse(TenantIds[i0]), TenantNames[i0]);

                    await _tenantRepository.InsertAsync(tenant);
                }
            }

        }

        internal class MyTenant : Tenant
        {
            internal MyTenant(Guid id, string name) : base(id, name)
            {
                //�̳���ſ��ԸĲ��� 
            }
        }

    }
}
