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
namespace HengDa.LiZongMing.REAMS.ZCQ.Dtos
{

    /// <summary>
    /// 气碘运行状态 ListDto
    /// </summary>
    //[AutoMapFrom(typeof(ZcqRunStatus))]
public partial class ZcqRunStatusListDto : EntityDto<Guid>
    {
        #region 构造方法
        public ZcqRunStatusListDto()
        {
        }
        #endregion


        #region 基本属性

        ///<summary>
        /// 设备Id
        ///</summary>
        [Display(Name = "设备Id", Order = 26)]
        public virtual long DeviceId { get; set; }


        ///<summary>
        /// 工作模式
        ///</summary>
        [MaxLength(256)]
        [Display(Name = "工作模式", Order = 27)]
        public virtual string WorkMode { get; set; }


        ///<summary>
        /// 设备状态
        ///</summary>
        [Display(Name = "设备状态", Order = 28)]
        public virtual ushort WorkStatus { get; set; }


        ///<summary>
        /// 设备状态描述
        ///</summary>
        [MaxLength(256)]
        [Display(Name = "设备状态描述", Order = 29)]
        public virtual string WorkStatusName { get; set; }


        ///<summary>
        /// 标况体积
        ///</summary>
        [Display(Name = "标况体积", Order = 30)]
        public virtual decimal StandardVolume { get; set; }


        ///<summary>
        /// 工况体积
        ///</summary>
        [Display(Name = "工况体积", Order = 31)]
        public virtual decimal WorkingVolume { get; set; }


        ///<summary>
        /// 舱门是否打开
        ///</summary>
        [Display(Name = "舱门是否打开", Order = 32)]
        public virtual bool IsDoorOpened { get; set; }


        ///<summary>
        /// LCD连接状态
        ///</summary>
        [Display(Name = "LCD连接状态", Order = 33)]
        public virtual bool LCDStatus { get; set; }


        ///<summary>
        /// 大气A是否超限
        ///</summary>
        [Display(Name = "大气A是否超限", Order = 34)]
        public virtual bool IsPressureAOverrun { get; set; }


        ///<summary>
        /// 大气B是否超限
        ///</summary>
        [Display(Name = "大气B是否超限", Order = 35)]
        public virtual bool IsPressureBOverrun { get; set; }


        ///<summary>
        /// 气碘是否超限
        ///</summary>
        [Display(Name = "气碘是否超限", Order = 36)]
        public virtual bool IsIodineOverrun { get; set; }


        ///<summary>
        /// A电子流量计FS4003故障
        ///</summary>
        [Display(Name = "A电子流量计FS4003故障", Order = 37)]
        public virtual bool FS4003AError { get; set; }


        ///<summary>
        /// B电子流量计FS4003故障
        ///</summary>
        [Display(Name = "B电子流量计FS4003故障", Order = 38)]
        public virtual bool FS4003BError { get; set; }


        ///<summary>
        /// 差压传感器故障
        ///</summary>
        [Display(Name = "差压传感器故障", Order = 39)]
        public virtual bool DpSensorError { get; set; }


        ///<summary>
        /// 大气压模块故障
        ///</summary>
        [Display(Name = "大气压模块故障", Order = 40)]
        public virtual bool AtmosphereModuleError { get; set; }


        ///<summary>
        /// 温度模块故障
        ///</summary>
        [Display(Name = "温度模块故障", Order = 41)]
        public virtual bool TemModuleError { get; set; }


        ///<summary>
        /// 瞬时采样流量设定值
        ///</summary>
        [Display(Name = "瞬时采样流量设定值", Order = 42)]
        public virtual int SettingFlow { get; set; }


        ///<summary>
        /// 已设定的采样时间的小时部分值
        ///</summary>
        [Display(Name = "已设定的采样时间的小时部分值", Order = 43)]
        public virtual int SettingHour { get; set; }


        ///<summary>
        /// 已设定的采样时间的分钟部分值
        ///</summary>
        [Display(Name = "已设定的采样时间的分钟部分值", Order = 44)]
        public virtual short SettingMin { get; set; }


        ///<summary>
        /// 已设定的采样体积值
        ///</summary>
        [Display(Name = "已设定的采样体积值", Order = 45)]
        public virtual decimal SettingTotalFlow { get; set; }


        ///<summary>
        /// 任务启动时间
        ///</summary>
        [Display(Name = "任务启动时间", Order = 46)]
        public virtual DateTime? TaskStartTime { get; set; }


        ///<summary>
        /// 创建时间
        ///</summary>
        [Display(Name = "创建时间", Order = 47)]
        public virtual DateTime CreationTime { get; set; }


        ///<summary>
        /// 状态更新时间
        ///</summary>
        [Display(Name = "状态更新时间", Order = 48)]
        public virtual DateTime? RunUpdateTime { get; set; }


        ///<summary>
        /// 警报更新时间
        ///</summary>
        [Display(Name = "警报更新时间", Order = 49)]
        public virtual DateTime? AlarmUpdateTime { get; set; }


        ///<summary>
        /// 多租户
        ///</summary>
        [Display(Name = "多租户", Order = 50)]
        public virtual Guid? TenantId { get; set; }


        #endregion

        #region 保留  手动添加属性

        #endregion


    }
}