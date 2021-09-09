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
using HengDa.LiZongMing.REAMS.ZCQ;
using HengDa.LiZongMing.REAMS.ZCQ.Dtos;

using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;



namespace HengDa.LiZongMing.REAMS.ZCQ
{

    /// <summary>
    /// 气碘运行状态 EditDto
    /// </summary>

    [Authorize(/*REAMSPermissions.ZcqRunStatus.Default*/)]
    public partial class ZcqRunStatusAppService : REAMSAppServiceBase<ZcqRunStatus, ZcqRunStatusDto, Guid, ZcqRunStatusPagedRequest, ZcqRunStatusDto, ZcqRunStatusDto>, IZcqRunStatusAppService
    {

        #region 构造方法

        //private readonly ZcqRunStatusManager _zcqRunStatusManager;

        public ZcqRunStatusAppService(
            IZcqRunStatusRepository repository  //mongodb必须用这个             IRepository<ZcqRunStatus, Guid> repository
            //ZcqRunStatusManager zcqRunStatusManager
            ):base(repository)
        {

            //权限控制
            //GetPolicyName = REAMSPermissions.ZcqRunStatus.Query;
            //GetListPolicyName = REAMSPermissions.ZcqRunStatus.Query;
            //CreatePolicyName = REAMSPermissions.ZcqRunStatus.Create;
            //UpdatePolicyName = REAMSPermissions.ZcqRunStatus.Edit;
            //DeletePolicyName = REAMSPermissions.ZcqRunStatus.Delete;

        }

        #endregion


        [Authorize(REAMSPermissions.ZcqRunStatus.Query)]
        public override Task<ZcqRunStatusDto> GetAsync(Guid id)
        {
            return base.GetAsync(id);
        }
        [Authorize(REAMSPermissions.ZcqRunStatus.Query)]
        public override Task<PagedResultDto<ZcqRunStatusDto>> GetListAsync(ZcqRunStatusPagedRequest input)
        {
            return base.GetListAsync(input);
        }
        /// <summary>
        /// 获取一个现有实体，或者新的默认实体
        /// </summary>
        [Authorize(REAMSPermissions.ZcqRunStatus.Create)]
        public async Task<ZcqRunStatusDto> GetOrNew(Nullable<Guid> id)
        {
            ZcqRunStatusDto editDto = null;

			if (id.HasValue)
			{
				var entity = await Repository.GetAsync(id.Value);
				editDto = MapToGetOutputDto(entity);
			}
			else
			{
				editDto = new ZcqRunStatusDto();
			}

			return editDto;
        }

        [Authorize(REAMSPermissions.ZcqRunStatus.Create)]
        public override async Task<ZcqRunStatusDto> CreateAsync(ZcqRunStatusDto input)
        {
            
            //CheckCreatePermission();
            return await base.CreateAsync(input);
            //var zcqRunStatus = MapToEntity(input);
            //return MapToGetOutputDto(zcqRunStatus);
        }

        [Authorize(REAMSPermissions.ZcqRunStatus.Edit)]
        public override async Task<ZcqRunStatusDto> UpdateAsync(Guid id, ZcqRunStatusDto input)
        {
            //CheckUpdatePermission();
            return await base.UpdateAsync(id,input);

            //var zcqRunStatus = await _zcqRunStatusManager.GetZcqRunStatusByIdAsync(input.Id);
            //MapToEntity(input, zcqRunStatus);
            //CheckErrors(await _zcqRunStatusManager.UpdateAsync(zcqRunStatus));
            //if (input.RoleNames != null)
            //{
            //    CheckErrors(await _zcqRunStatusManager.SetRoles(zcqRunStatus, input.RoleNames));
            //}
            //return await Get(input);
        }

        [Authorize(REAMSPermissions.ZcqRunStatus.Delete)]
        public override  Task DeleteAsync(Guid id)
        {
            return base.DeleteAsync(id);
            //var zcqRunStatus = await _zcqRunStatusManager.GetZcqRunStatusByIdAsync(id);
            //await _zcqRunStatusManager.DeleteAsync(zcqRunStatus);
        }

        protected override async Task<ZcqRunStatus> GetEntityByIdAsync(Guid id)
        {
            return await base.GetEntityByIdAsync(id);
            //var zcqRunStatus = await Repository.GetAllIncluding(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);
            //if (zcqRunStatus == null)
            //{
            //    throw new EntityNotFoundException(typeof(ZcqRunStatus), id);
            //}
            //return zcqRunStatus;
            //return await Repository.GetAsync(id);
        }

        #region   IObjectMapper实体转换方法
        /*
        /// <summary>
        /// 读取单个实体 <see cref="ZcqRunStatus"/> 时转换为 <see cref="ZcqRunStatusDto"/>.
        /// It uses <see cref="IObjectMapper"/> by default.
        /// </summary>
        protected virtual ZcqRunStatusDto MapToGetOutputDto(ZcqRunStatus entity)
        {
            //return ObjectMapper.Map<ZcqRunStatus, ZcqRunStatusDto>(entity);
            //return base.MapToGetOutputDto(entity);
        }

        /// <summary>
        /// 读取列表实体 <see cref="ZcqRunStatus"/> 时转换为 <see cref="ZcqRunStatusDto"/>.
        /// It uses <see cref="IObjectMapper"/> by default.
        /// </summary>
        protected virtual PagedResultDto<ZcqRunStatusDto> MapToGetListOutputDto(TEntity entity)
        {
            //return ObjectMapper.Map<TEntity, TGetListOutputDto>(entity);
            //return base.MapToGetListOutputDto(entity);
        }

        /// <summary>
        /// 新增时将参数转换成实体
        /// </summary>
        protected override ZcqRunStatus MapToEntity(ZcqRunStatusDto createInput)
        {
            return base.MapToEntity(createInput);
            //var zcqRunStatus = ObjectMapper.Map<ZcqRunStatus>(createInput);
            //zcqRunStatus.SetNormalizedNames();
            //return zcqRunStatus;
        }

        /// <summary>
        /// 编辑时将参数转换成实体
        /// </summary>
        protected override void MapToEntity(ZcqRunStatusDto updateInput, ZcqRunStatus zcqRunStatus)
        {
            base.MapToEntity(updateInput, zcqRunStatus);
            //ObjectMapper.Map(updateInput, zcqRunStatus);
            //zcqRunStatus.SetNormalizedNames();
        }
        */
        #endregion

        #region   不保留  条件过滤
        protected override Task<IQueryable<ZcqRunStatus>> CreateFilteredQueryAsync(ZcqRunStatusPagedRequest input)
        {
            var q = Repository
                //.AsNoTracking()
                //.WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Title.Contains(input.Keyword) )

                 .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.WorkStatusName.Contains(input.Keyword)
                        )
                 .WhereIf(input.DeviceId.HasValue, x => x.DeviceId == input.DeviceId)
                 .WhereIf(!input.WorkStatusName.IsNullOrWhiteSpace(), x => x.WorkStatusName.Contains(input.WorkStatusName))
                 .WhereIf(input.TimeStart.HasValue, x => x.CreationTime >= input.TimeStart)
                 .WhereIf(input.TimeEnd.HasValue, x => x.CreationTime < input.TimeEnd) 
                 .WhereIf(input.TenantId.HasValue, x => x.TenantId == input.TenantId)
                ;
            return Task.FromResult(q);
        }
        #endregion

        #region   不保留 排序
        protected override IQueryable<ZcqRunStatus> ApplySorting(IQueryable<ZcqRunStatus> query, ZcqRunStatusPagedRequest input)
        {
            return base.ApplySorting(query, input);
            //return query.OrderByDescending(r => r.Id);
        }
        #endregion

        #region 不保留  手动添加

        #endregion


    }
}