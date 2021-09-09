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

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Volo.Abp.Application.Dtos;

using HengDa.LiZongMing.REAMS;
namespace HengDa.LiZongMing.REAMS.ZJC.Dtos
{

    /// <summary>
    /// 湿沉降降雨记录 Dto
    /// </summary>
    //[AutoMapFrom(typeof(ZjcRainRecord))]
public partial class ZjcRainRecordDto : EntityDto<Guid>
    {
        #region 构造方法
        public ZjcRainRecordDto()
        {
        }
        #endregion


        #region 基本属性

        ///<summary>
        /// 设备Id
        ///</summary>
        [Display(Name = "设备Id", Order = 1)]
        public virtual long DeviceId { get; set; }


        ///<summary>
        /// 降雨开始时间
        ///</summary>
        [Display(Name = "降雨开始时间", Order = 2)]
        public virtual DateTime StartTime { get; set; }


        ///<summary>
        /// 降雨结束时间
        ///</summary>
        [Display(Name = "降雨结束时间", Order = 3)]
        public virtual DateTime EndTime { get; set; }


        ///<summary>
        /// 降雨量
        ///</summary>
        [Display(Name = "降雨量", Order = 4)]
        public virtual decimal Rainfall { get; set; }


        ///<summary>
        /// 降雨强度
        ///</summary>
        [Display(Name = "降雨强度", Order = 5)]
        public virtual decimal Intensity { get; set; }


        ///<summary>
        /// 创建时间
        ///</summary>
        [Display(Name = "创建时间", Order = 6)]
        public virtual DateTime CreationTime { get; set; }


        ///<summary>
        /// 多租户
        ///</summary>
        [Display(Name = "多租户", Order = 7)]
        public virtual Guid? TenantId { get; set; }


        #endregion

        #region 保留  手动添加属性

        #endregion



    }
}