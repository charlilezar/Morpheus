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
namespace HengDa.LiZongMing.REAMS.HVE.Dtos
{

    /// <summary>
    /// 高压电离伽马记录 Dto
    /// </summary>
    //[AutoMapFrom(typeof(HveRecord))]
public partial class HveRecordDto : EntityDto<Guid>
    {
        #region 构造方法
        public HveRecordDto()
        {
        }
        #endregion


        #region 基本属性

        // ///<summary>
        // /// Id
        // ///</summary>
        // [Display(Name = "Id", Order = 1)]
        // public virtual Guid Id { get; set; }


        ///<summary>
        /// 创建时间
        ///</summary>
        [Display(Name = "创建时间", Order = 2)]
        public virtual DateTime CreationTime { get; set; }


        ///<summary>
        /// 设备Id
        ///</summary>
        [Display(Name = "设备Id", Order = 3)]
        public virtual long DeviceId { get; set; }


        ///<summary>
        /// 伽马剂量率
        ///</summary>
        [Display(Name = "伽马剂量率", Order = 4)]
        public virtual decimal DoseRate { get; set; }


        ///<summary>
        /// 高电压
        ///</summary>
        [Display(Name = "高电压", Order = 5)]
        public virtual decimal HighVoltage { get; set; }


        ///<summary>
        /// 静电计温度
        ///</summary>
        [Display(Name = "静电计温度", Order = 6)]
        public virtual decimal ElectrometerTemperature { get; set; }


        ///<summary>
        /// 电池电压
        ///</summary>
        [Display(Name = "电池电压", Order = 7)]
        public virtual decimal BatteryVoltage { get; set; }


        ///<summary>
        /// 多租户
        ///</summary>
        [Display(Name = "多租户", Order = 8)]
        public virtual Guid? TenantId { get; set; }


        #endregion

        #region 保留  手动添加属性

        #endregion


    }
}