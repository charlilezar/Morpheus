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
    /// 干湿沉降实时数据 ListDto
    /// </summary>
    //[AutoMapFrom(typeof(ZjcRunStatus))]
public partial class ZjcRunStatusListDto : EntityDto<Guid>
    {
        #region 构造方法
        public ZjcRunStatusListDto()
        {
        }
        #endregion


        #region 基本属性

        ///<summary>
        /// 设备Id
        ///</summary>
        [Display(Name = "设备Id", Order = 27)]
        public virtual long DeviceId { get; set; }


        ///<summary>
        /// 干沉降总条数
        ///</summary>
        [Display(Name = "干沉降总条数", Order = 28)]
        public virtual ushort DryDepositionCount { get; set; }


        ///<summary>
        /// 当前仪器状态
        ///</summary>
        [Display(Name = "当前仪器状态", Order = 29)]
        public virtual ushort Status { get; set; }


        ///<summary>
        /// 设备实时时钟
        ///</summary>
        [Display(Name = "设备实时时钟", Order = 30)]
        public virtual DateTime InstrumentTime { get; set; }


        ///<summary>
        /// 前一天降雨总量
        ///</summary>
        [Display(Name = "前一天降雨总量", Order = 31)]
        public virtual decimal RainfallYesterday { get; set; }


        ///<summary>
        /// 当前逢雨雨量
        ///</summary>
        [Display(Name = "当前逢雨雨量", Order = 32)]
        public virtual decimal RainfallCurrent { get; set; }


        ///<summary>
        /// 环境温度
        ///</summary>
        [Display(Name = "环境温度", Order = 33)]
        public virtual decimal Temperature { get; set; }


        ///<summary>
        /// 湿度
        ///</summary>
        [Display(Name = "湿度", Order = 34)]
        public virtual decimal Humidity { get; set; }


        ///<summary>
        /// 机箱温度
        ///</summary>
        [Display(Name = "机箱温度", Order = 35)]
        public virtual decimal TempOfBox { get; set; }


        ///<summary>
        /// 采样桶温度
        ///</summary>
        [Display(Name = "采样桶温度", Order = 36)]
        public virtual decimal TempOfBucket { get; set; }


        ///<summary>
        /// 雨感器温度
        ///</summary>
        [Display(Name = "雨感器温度", Order = 37)]
        public virtual decimal TempOfRainSensor { get; set; }


        ///<summary>
        /// 查询日期内降雨总时间
        ///</summary>
        [Display(Name = "查询日期内降雨总时间", Order = 38)]
        public virtual ushort Raintimes { get; set; }


        ///<summary>
        /// 集雨桶水满
        ///</summary>
        [Display(Name = "集雨桶水满", Order = 39)]
        public virtual bool FilledWater { get; set; }


        ///<summary>
        /// 开盖超限
        ///</summary>
        [Display(Name = "开盖超限", Order = 40)]
        public virtual bool AlarmLidOpenedOver { get; set; }


        ///<summary>
        /// 关盖超限
        ///</summary>
        [Display(Name = "关盖超限", Order = 41)]
        public virtual bool AlarmLidClosedOver { get; set; }


        ///<summary>
        /// 机箱温控故障
        ///</summary>
        [Display(Name = "机箱温控故障", Order = 42)]
        public virtual bool AlarmBoxTemp { get; set; }


        ///<summary>
        /// 采样桶温控故障
        ///</summary>
        [Display(Name = "采样桶温控故障", Order = 43)]
        public virtual bool AlarmBucketTemp { get; set; }


        ///<summary>
        /// 感雨器温控故障
        ///</summary>
        [Display(Name = "感雨器温控故障", Order = 44)]
        public virtual bool AlarmRainSensor { get; set; }


        ///<summary>
        /// 干沉降桶液位满故障
        ///</summary>
        [Display(Name = "干沉降桶液位满故障", Order = 45)]
        public virtual bool AlarmDryBucketFilled { get; set; }


        ///<summary>
        /// 干沉降桶液位低故障
        ///</summary>
        [Display(Name = "干沉降桶液位低故障", Order = 46)]
        public virtual bool AlarmDryBucketWaterLess { get; set; }


        ///<summary>
        /// 集雨器水满故障
        ///</summary>
        [Display(Name = "集雨器水满故障", Order = 47)]
        public virtual bool AlarmJyqFilled { get; set; }


        ///<summary>
        /// 环境温度故障
        ///</summary>
        [Display(Name = "环境温度故障", Order = 48)]
        public virtual bool AlarmTemperature { get; set; }


        ///<summary>
        /// 创建时间
        ///</summary>
        [Display(Name = "创建时间", Order = 49)]
        public virtual DateTime CreationTime { get; set; }


        ///<summary>
        /// 状态更新时间
        ///</summary>
        [Display(Name = "状态更新时间", Order = 50)]
        public virtual DateTime? RunUpdateTime { get; set; }


        ///<summary>
        /// 警报更新时间
        ///</summary>
        [Display(Name = "警报更新时间", Order = 51)]
        public virtual DateTime? AlarmUpdateTime { get; set; }


        ///<summary>
        /// 多租户
        ///</summary>
        [Display(Name = "多租户", Order = 52)]
        public virtual Guid? TenantId { get; set; }


        #endregion

        #region 保留  手动添加属性

        #endregion


    }
}