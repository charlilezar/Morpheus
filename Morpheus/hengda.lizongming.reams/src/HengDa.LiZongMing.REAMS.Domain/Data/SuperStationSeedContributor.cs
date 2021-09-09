using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HengDa.LiZongMing.REAMS.Devices;
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
using Volo.Abp.Uow;
using ApiResource = Volo.Abp.IdentityServer.ApiResources.ApiResource;
using ApiScope = Volo.Abp.IdentityServer.ApiScopes.ApiScope;
using Client = Volo.Abp.IdentityServer.Clients.Client;

namespace HengDa.LiZongMing.REAMS.IdentityServer
{
    public class SuperStationSeedContributor : IDataSeedContributor, ITransientDependency
    {

        private readonly IGuidGenerator _guidGenerator;
        private readonly IPermissionDataSeeder _permissionDataSeeder;
        private readonly IConfiguration _configuration;
        private readonly ICurrentTenant _currentTenant;
        private readonly IDeviceRepository deviceRepository;

        public SuperStationSeedContributor(

            IGuidGenerator guidGenerator,
            IPermissionDataSeeder permissionDataSeeder,
            IConfiguration configuration,
            IDeviceRepository deviceRepository,
            ICurrentTenant currentTenant)
        {

            _guidGenerator = guidGenerator;
            _permissionDataSeeder = permissionDataSeeder;
            _configuration = configuration;
            this.deviceRepository = deviceRepository;

            _currentTenant = currentTenant;
        }

        [UnitOfWork]
        public virtual async Task SeedAsync(DataSeedContext context)
        {
            using (_currentTenant.Change(context?.TenantId))
            {

                await CreateDrivesAsync();
            }
        }
        private async Task CreateDrivesAsync()
        {
            //var devices = await deviceRepository.GetListAsync();

            var names = "动环监测设备_3,动环监测设备_4".Split(',');
            foreach (var name in names)
            {
                if (await deviceRepository.FindAsync(m => m.Name == name) == null)
                {
                    var device = new Device()
                    {
                        Name = name,
                        GeoLocation_Altitude = 0,
                        GeoLocation_Lat = 30,
                        GeoLocation_Lng = 120,
                        HWConnectionJson = "{}",
                        ManufactureDate = DateTime.Today,
                        MqttUserName = "88888888",
                        MqttPassword = "88888888",
                        ProductName = name,
                        ProductModel = name + "型号",
                        SN = "88888888",
                        TestAddress = "在测地址",
                        //CustomerService="售后工程师1",
                        IsEnable = true,
                        TenantId = _currentTenant.GetId()

                    };

                   await deviceRepository.InsertAsync(device);
                }
            }





        }

    }
}
