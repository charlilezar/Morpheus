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
using HengDa.LiZongMing.REAMS.NBS;
using HengDa.LiZongMing.REAMS.NBS.Dtos;

using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;



namespace HengDa.LiZongMing.REAMS.NBS
{

    /// <summary>
    /// 超大气溶胶采样记录 EditDto
    /// </summary>

    [Authorize(/*REAMSPermissions.NbsRecord.Default*/)]
    public partial class NbsRecordAppService : REAMSAppServiceBase<NbsRecord, NbsRecordDto, Guid, NbsRecordPagedRequest, NbsRecordDto, NbsRecordDto>, INbsRecordAppService
    {

        #region 构造方法

        //private readonly NbsRecordManager _nbsRecordManager;

        public NbsRecordAppService(
            INbsRecordRepository repository  //mongodb必须用这个             IRepository<NbsRecord, Guid> repository
            //NbsRecordManager nbsRecordManager
            ):base(repository)
        {

            //权限控制
            //GetPolicyName = REAMSPermissions.NbsRecord.Query;
            //GetListPolicyName = REAMSPermissions.NbsRecord.Query;
            //CreatePolicyName = REAMSPermissions.NbsRecord.Create;
            //UpdatePolicyName = REAMSPermissions.NbsRecord.Edit;
            //DeletePolicyName = REAMSPermissions.NbsRecord.Delete;

        }

        #endregion


        [Authorize(REAMSPermissions.NbsRecord.Query)]
        public override Task<NbsRecordDto> GetAsync(Guid id)
        {
            return base.GetAsync(id);
        }
        [Authorize(REAMSPermissions.NbsRecord.Query)]
        public override Task<PagedResultDto<NbsRecordDto>> GetListAsync(NbsRecordPagedRequest input)
        {
            return base.GetListAsync(input);
        }
        /// <summary>
        /// 获取一个现有实体，或者新的默认实体
        /// </summary>
        [Authorize(REAMSPermissions.NbsRecord.Create)]
        public async Task<NbsRecordDto> GetOrNew(Nullable<Guid> id)
        {
            NbsRecordDto editDto = null;

			if (id.HasValue)
			{
				var entity = await Repository.GetAsync(id.Value);
				editDto = MapToGetOutputDto(entity);
			}
			else
			{
				editDto = new NbsRecordDto();
			}

			return editDto;
        }

        [Authorize(REAMSPermissions.NbsRecord.Create)]
        public override async Task<NbsRecordDto> CreateAsync(NbsRecordDto input)
        {
            //CheckCreatePermission();
            return await base.CreateAsync(input);
            //var nbsRecord = MapToEntity(input);
            //return MapToGetOutputDto(nbsRecord);
        }

        [Authorize(REAMSPermissions.NbsRecord.Edit)]
        public override async Task<NbsRecordDto> UpdateAsync(Guid id, NbsRecordDto input)
        {
            //CheckUpdatePermission();
            return await base.UpdateAsync(id,input);

            //var nbsRecord = await _nbsRecordManager.GetNbsRecordByIdAsync(input.Id);
            //MapToEntity(input, nbsRecord);
            //CheckErrors(await _nbsRecordManager.UpdateAsync(nbsRecord));
            //if (input.RoleNames != null)
            //{
            //    CheckErrors(await _nbsRecordManager.SetRoles(nbsRecord, input.RoleNames));
            //}
            //return await Get(input);
        }

        [Authorize(REAMSPermissions.NbsRecord.Delete)]
        public override  Task DeleteAsync(Guid id)
        {
            return base.DeleteAsync(id);
            //var nbsRecord = await _nbsRecordManager.GetNbsRecordByIdAsync(id);
            //await _nbsRecordManager.DeleteAsync(nbsRecord);
        }

        protected override async Task<NbsRecord> GetEntityByIdAsync(Guid id)
        {
            return await base.GetEntityByIdAsync(id);
            //var nbsRecord = await Repository.GetAllIncluding(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);
            //if (nbsRecord == null)
            //{
            //    throw new EntityNotFoundException(typeof(NbsRecord), id);
            //}
            //return nbsRecord;
            //return await Repository.GetAsync(id);
        }

        #region   IObjectMapper实体转换方法
        /*
        /// <summary>
        /// 读取单个实体 <see cref="NbsRecord"/> 时转换为 <see cref="NbsRecordDto"/>.
        /// It uses <see cref="IObjectMapper"/> by default.
        /// </summary>
        protected virtual NbsRecordDto MapToGetOutputDto(NbsRecord entity)
        {
            //return ObjectMapper.Map<NbsRecord, NbsRecordDto>(entity);
            //return base.MapToGetOutputDto(entity);
        }

        /// <summary>
        /// 读取列表实体 <see cref="NbsRecord"/> 时转换为 <see cref="NbsRecordDto"/>.
        /// It uses <see cref="IObjectMapper"/> by default.
        /// </summary>
        protected virtual PagedResultDto<NbsRecordDto> MapToGetListOutputDto(TEntity entity)
        {
            //return ObjectMapper.Map<TEntity, TGetListOutputDto>(entity);
            //return base.MapToGetListOutputDto(entity);
        }

        /// <summary>
        /// 新增时将参数转换成实体
        /// </summary>
        protected override NbsRecord MapToEntity(NbsRecordDto createInput)
        {
            return base.MapToEntity(createInput);
            //var nbsRecord = ObjectMapper.Map<NbsRecord>(createInput);
            //nbsRecord.SetNormalizedNames();
            //return nbsRecord;
        }

        /// <summary>
        /// 编辑时将参数转换成实体
        /// </summary>
        protected override void MapToEntity(NbsRecordDto updateInput, NbsRecord nbsRecord)
        {
            base.MapToEntity(updateInput, nbsRecord);
            //ObjectMapper.Map(updateInput, nbsRecord);
            //nbsRecord.SetNormalizedNames();
        }
        */
        #endregion

        #region   不保留  条件过滤
        protected override Task<IQueryable<NbsRecord>> CreateFilteredQueryAsync(NbsRecordPagedRequest input)
        {
            var q = Repository
                //.AsNoTracking()
                //.WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Title.Contains(input.Keyword) )

                 .WhereIf(input.DeviceId.HasValue, x => x.DeviceId == input.DeviceId)
                 .WhereIf(!input.BatchNumber.IsNullOrWhiteSpace(), x => x.BatchNumber == input.BatchNumber)
                 .WhereIf(input.TimeStart.HasValue, x => x.CreationTime >= input.TimeStart)
                 .WhereIf(input.TimeEnd.HasValue, x => x.CreationTime < input.TimeEnd) 
                 .WhereIf(input.TenantId.HasValue, x => x.TenantId == input.TenantId)
                ;
            return Task.FromResult(q);
        }
        #endregion

        #region   不保留 排序
        protected override IQueryable<NbsRecord> ApplySorting(IQueryable<NbsRecord> query, NbsRecordPagedRequest input)
        {
            return base.ApplySorting(query, input);
            //return query.OrderByDescending(r => r.Id);
        }
        #endregion

        #region 不保留  手动添加

        #endregion


    }
}