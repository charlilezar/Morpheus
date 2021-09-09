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
    public class DataSeedContributor : IDataSeedContributor, ITransientDependency
    {

        private readonly IGuidGenerator _guidGenerator;
        private readonly IPermissionDataSeeder _permissionDataSeeder;
        private readonly IConfiguration _configuration;
        private readonly ICurrentTenant _currentTenant;
        private readonly IDeviceRepository deviceRepository;
        private readonly IDeviceRoomRepository deviceRoomRepository;

        public DataSeedContributor(

            IGuidGenerator guidGenerator,
            IPermissionDataSeeder permissionDataSeeder,
            IConfiguration configuration,
            IDeviceRepository deviceRepository,
            IDeviceRoomRepository deviceRoomRepository,
            ICurrentTenant currentTenant)
        {

            _guidGenerator = guidGenerator;
            _permissionDataSeeder = permissionDataSeeder;
            _configuration = configuration;
            this.deviceRepository = deviceRepository;
            this.deviceRoomRepository = deviceRoomRepository;

            _currentTenant = currentTenant;
        }

        [UnitOfWork]
        public virtual async Task SeedAsync(DataSeedContext context)
        {
            using (_currentTenant.Change(context?.TenantId))
            {
                if (_currentTenant.Id.HasValue && _currentTenant.GetId() == Guid.Parse("39f99bf5-c4eb-33d2-a623-591eec290001")) //只在特定的内测租户内添加
                {
                    await CreateDeviceRoomAsync();
                    await CreateDrivesAsync();
                }
            }
        }
        private async Task CreateDeviceRoomAsync()
        {


            for (int i = 1; i <= 2; i++)
            {

                if (await deviceRoomRepository.FindAsync(m => m.Id == i) == null)
                {
                    var deviceRoom = new MyDeviceRoom(i)
                    {
                        Name = i.ToString() + "号站房",
                        GeoLocation_Altitude = 0,
                        GeoLocation_Lat = 30,
                        GeoLocation_Lng = 120,
                        HWConnectionJson = "{}",
                        ManufactureDate = DateTime.Today,
                        MqttUserName = "88888888",
                        MqttPassword = "88888888",

                        ProductModel = "综合站房",
                        SN = "88888888",
                        TestAddress = "在测地址",
                        //CustomerService="售后工程师1",
                        IsEnable = true,

                        //TenantId = _currentTenant.GetId()

                    };
                    if (_currentTenant.Id.HasValue)
                        deviceRoom.TenantId = _currentTenant.GetId();

                    await deviceRoomRepository.InsertAsync(deviceRoom);
                }
            }
        }
        private async Task CreateDrivesAsync()
        {
            //var devices = await deviceRepository.GetListAsync();

            var names = "站房环境,超大流量采样器,高压电离室,碘化纳普仪,大气碘采样器,干湿沉降采样器,降雨感应器,自动气象仪".Split(',');
            foreach (var name in names)
            {
                if (await deviceRepository.FindAsync(m => m.Name == name) == null)
                {
                    var device = new Device()
                    {
                        DeviceRoomId = 1,
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

                        //TenantId = _currentTenant.GetId()

                    };
                    if (_currentTenant.Id.HasValue)
                        device.TenantId = _currentTenant.GetId();

                    await deviceRepository.InsertAsync(device);
                }
            }

        }


        class MyDeviceRoom : DeviceRoom
        {
            internal MyDeviceRoom(long Id)
            {
                base.Id = Id;
            }
        }

    }

}

