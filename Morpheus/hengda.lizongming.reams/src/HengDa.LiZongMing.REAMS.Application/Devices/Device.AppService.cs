﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成 by 明教主加强的代码生成模板( 作者 m8989@qq.com) 。
//     T4模板 + 阿明加强的代码生成模板
//  
//     对此文件的更改可能会导致不正确的行为。此外，如果重新生成代码，这些更改将会丢失。
//      所以如有修改的必要，建议建立 partial 类进行增加自己的代码
//                              或使用 #region 保留  功能说明  和 #endregion 来包围
// </auto-generated>
//------------------------------------------------------------------------------

using HengDa.LiZongMing.REAMS;
using HengDa.LiZongMing.REAMS.Authorization;
using HengDa.LiZongMing.REAMS.Devices;
using HengDa.LiZongMing.REAMS.Devices.Dtos;

using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;



namespace HengDa.LiZongMing.REAMS.Devices
{

    /// <summary>
    /// 设备信息 EditDto
    /// </summary>

    [Authorize(/*REAMSPermissions.Device.Default*/)]
    public partial class DeviceAppService : REAMSAppServiceBase<Device, DeviceDto, long, DevicePagedRequest, DeviceDto, DeviceDto>, IDeviceAppService
    {

        #region 构造方法

        //private readonly DeviceManager _deviceManager;

        public DeviceAppService(
            IDeviceRepository repository  //mongodb必须用这个             IRepository<Device, long> repository
            //DeviceManager deviceManager
            ):base(repository)
        {

            //权限控制
            //GetPolicyName = REAMSPermissions.Device.Query;
            //GetListPolicyName = REAMSPermissions.Device.Query;
            //CreatePolicyName = REAMSPermissions.Device.Create;
            //UpdatePolicyName = REAMSPermissions.Device.Edit;
            //DeletePolicyName = REAMSPermissions.Device.Delete;

        }

        #endregion


        [Authorize(REAMSPermissions.Device.Query)]
        public override Task<DeviceDto> GetAsync(long id)
        {
            return base.GetAsync(id);
        }
        [Authorize(REAMSPermissions.Device.Query)]
        public override Task<PagedResultDto<DeviceDto>> GetListAsync(DevicePagedRequest input)
        {
            return base.GetListAsync(input);
        }
        /// <summary>
        /// 获取一个现有实体，或者新的默认实体
        /// </summary>
        [Authorize(REAMSPermissions.Device.Create)]
        public async Task<DeviceDto> GetOrNew(Nullable<long> id)
        {
            DeviceDto editDto = null;

			if (id.HasValue && id.Value > 0 )
			{
				var entity = await Repository.GetAsync(id.Value);
				editDto = MapToGetOutputDto(entity);
			}
			else
			{
				editDto = new DeviceDto();
			}

			return editDto;
        }

        [Authorize(REAMSPermissions.Device.Create)]
        public override async Task<DeviceDto> CreateAsync(DeviceDto input)
        {
            //CheckCreatePermission();
            return await base.CreateAsync(input);
            //var device = MapToEntity(input);
            //return MapToGetOutputDto(device);
        }

        [Authorize(REAMSPermissions.Device.Edit)]
        public override async Task<DeviceDto> UpdateAsync(long id, DeviceDto input)
        {
            //CheckUpdatePermission();
            return await base.UpdateAsync(id,input);

            //var device = await _deviceManager.GetDeviceByIdAsync(input.Id);
            //MapToEntity(input, device);
            //CheckErrors(await _deviceManager.UpdateAsync(device));
            //if (input.RoleNames != null)
            //{
            //    CheckErrors(await _deviceManager.SetRoles(device, input.RoleNames));
            //}
            //return await Get(input);
        }

        [Authorize(REAMSPermissions.Device.Delete)]
        public override  Task DeleteAsync(long id)
        {
            return base.DeleteAsync(id);
            //var device = await _deviceManager.GetDeviceByIdAsync(id);
            //await _deviceManager.DeleteAsync(device);
        }

        protected override async Task<Device> GetEntityByIdAsync(long id)
        {
            return await base.GetEntityByIdAsync(id);
            //var device = await Repository.GetAllIncluding(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);
            //if (device == null)
            //{
            //    throw new EntityNotFoundException(typeof(Device), id);
            //}
            //return device;
            //return await Repository.GetAsync(id);
        }

        #region   IObjectMapper实体转换方法
        /*
        /// <summary>
        /// 读取单个实体 <see cref="Device"/> 时转换为 <see cref="DeviceDto"/>.
        /// It uses <see cref="IObjectMapper"/> by default.
        /// </summary>
        protected virtual DeviceDto MapToGetOutputDto(Device entity)
        {
            //return ObjectMapper.Map<Device, DeviceDto>(entity);
            //return base.MapToGetOutputDto(entity);
        }

        /// <summary>
        /// 读取列表实体 <see cref="Device"/> 时转换为 <see cref="DeviceDto"/>.
        /// It uses <see cref="IObjectMapper"/> by default.
        /// </summary>
        protected virtual PagedResultDto<DeviceDto> MapToGetListOutputDto(TEntity entity)
        {
            //return ObjectMapper.Map<TEntity, TGetListOutputDto>(entity);
            //return base.MapToGetListOutputDto(entity);
        }

        /// <summary>
        /// 新增时将参数转换成实体
        /// </summary>
        protected override Device MapToEntity(DeviceDto createInput)
        {
            return base.MapToEntity(createInput);
            //var device = ObjectMapper.Map<Device>(createInput);
            //device.SetNormalizedNames();
            //return device;
        }

        /// <summary>
        /// 编辑时将参数转换成实体
        /// </summary>
        protected override void MapToEntity(DeviceDto updateInput, Device device)
        {
            base.MapToEntity(updateInput, device);
            //ObjectMapper.Map(updateInput, device);
            //device.SetNormalizedNames();
        }
        */
        #endregion

        #region   不保留  条件过滤
        protected override Task<IQueryable<Device>> CreateFilteredQueryAsync(DevicePagedRequest input)
        {
            var q = Repository
                //.AsNoTracking()
                //.WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Title.Contains(input.Keyword) )

                 .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword)
                        || x.MqttUserName.Contains(input.Keyword)
                        || x.ProductName.Contains(input.Keyword)
                        || x.ProductModel.Contains(input.Keyword)
                        || x.HWConnectionJson.Contains(input.Keyword)
                        || x.Remark.Contains(input.Keyword)
                        )
                 .WhereIf(input.Id.HasValue, x => x.Id == input.Id)
                 .WhereIf(input.DeviceRoomId.HasValue, x => x.DeviceRoomId == input.DeviceRoomId)
                 .WhereIf(input.CustomerId.HasValue, x => x.CustomerId == input.CustomerId)
                 .WhereIf(!input.Name.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Name))
                 .WhereIf(!input.MqttUserName.IsNullOrWhiteSpace(), x => x.MqttUserName.Contains(input.MqttUserName))
                 .WhereIf(!input.SN.IsNullOrWhiteSpace(), x => x.SN == input.SN)
                 .WhereIf(!input.ProductName.IsNullOrWhiteSpace(), x => x.ProductName.Contains(input.ProductName))
                 .WhereIf(input.TenantId.HasValue, x => x.TenantId == input.TenantId)
                ;
            return Task.FromResult(q);
        }
        #endregion

        #region   不保留 排序
        protected override IQueryable<Device> ApplySorting(IQueryable<Device> query, DevicePagedRequest input)
        {
            return base.ApplySorting(query, input);
            //return query.OrderByDescending(r => r.Id);
        }
        #endregion

        #region 不保留  手动添加

        #endregion


    }
}