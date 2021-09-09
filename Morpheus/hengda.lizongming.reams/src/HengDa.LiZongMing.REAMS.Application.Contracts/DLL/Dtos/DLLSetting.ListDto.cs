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
namespace HengDa.LiZongMing.REAMS.DLL.Dtos
{

    /// <summary>
    /// DLL设备参数信息 ListDto
    /// </summary>
    //[AutoMapFrom(typeof(DllSetting))]
public partial class DllSettingListDto : EntityDto<Guid>
    {
        #region 构造方法
        public DllSettingListDto()
        {
        }
        #endregion


        #region 基本属性

        // ///<summary>
        // /// Id
        // ///</summary>
        // [Display(Name = "Id", Order = 26)]
        // public virtual Guid Id { get; set; }


        ///<summary>
        /// 命令动作
        ///</summary>
        [MaxLength(256)]
        [Display(Name = "命令动作", Order = 27)]
        public virtual string Act { get; set; }


        ///<summary>
        /// 设备Id
        ///</summary>
        [Display(Name = "设备Id", Order = 28)]
        public virtual long DeviceId { get; set; }


        ///<summary>
        /// 地理位置-经度
        ///</summary>
        [Display(Name = "地理位置-经度", Order = 29)]
        public virtual decimal GeoLocation_Lng { get; set; }


        ///<summary>
        /// 地理位置-伟度
        ///</summary>
        [Display(Name = "地理位置-伟度", Order = 30)]
        public virtual decimal GeoLocation_Lat { get; set; }


        ///<summary>
        /// 累计运行时间 
        ///</summary>
        [Display(Name = "累计运行时间 ", Order = 31)]
        public virtual long Runtime { get; set; }


        ///<summary>
        /// 工作模式
        ///</summary>
        [Display(Name = "工作模式", Order = 32)]
        public virtual int WorkMode { get; set; }


        ///<summary>
        /// 设定流量
        ///</summary>
        [Display(Name = "设定流量", Order = 33)]
        public virtual decimal SettingFlow { get; set; }


        ///<summary>
        /// 采样次数
        ///</summary>
        [Display(Name = "采样次数", Order = 34)]
        public virtual int SampleCount { get; set; }


        ///<summary>
        /// 采样时间
        ///</summary>
        [Display(Name = "采样时间", Order = 35)]
        public virtual int SampleTime { get; set; }


        ///<summary>
        /// 采样间隔
        ///</summary>
        [Display(Name = "采样间隔", Order = 36)]
        public virtual int SampleTimespan { get; set; }


        ///<summary>
        /// 采样总量
        ///</summary>
        [Display(Name = "采样总量", Order = 37)]
        public virtual decimal SampleTotalVolume { get; set; }


        ///<summary>
        /// 测量间隔时间
        ///</summary>
        [Display(Name = "测量间隔时间", Order = 38)]
        public virtual decimal TestTimespan { get; set; }


        ///<summary>
        /// 采样结束后测量时间
        ///</summary>
        [Display(Name = "采样结束后测量时间", Order = 39)]
        public virtual decimal ExtraTime { get; set; }


        ///<summary>
        /// 测量读数次数
        ///</summary>
        [Display(Name = "测量读数次数", Order = 40)]
        public virtual int TestRecordCount { get; set; }


        ///<summary>
        /// 测量读数间隔
        ///</summary>
        [Display(Name = "测量读数间隔", Order = 41)]
        public virtual long TestRecordTimespan { get; set; }


        ///<summary>
        /// β/α最大预警值
        ///</summary>
        [Display(Name = "β/α最大预警值", Order = 42)]
        public virtual decimal MaxRatio { get; set; }


        ///<summary>
        /// 修正系数alpha
        ///</summary>
        [Display(Name = "修正系数alpha", Order = 43)]
        public virtual decimal alphaAdd { get; set; }


        ///<summary>
        /// 修正系数beta
        ///</summary>
        [Display(Name = "修正系数beta", Order = 44)]
        public virtual decimal betaAdd { get; set; }


        ///<summary>
        /// 测量时是否采样
        ///</summary>
        [Display(Name = "测量时是否采样", Order = 45)]
        public virtual bool TestAndSampling { get; set; }


        ///<summary>
        /// 任务启动时间
        ///</summary>
        [Display(Name = "任务启动时间", Order = 46)]
        public virtual DateTime? TaskStartTime { get; set; }


        ///<summary>
        /// 接收信号强度 
        ///</summary>
        [Display(Name = "接收信号强度 ", Order = 47)]
        public virtual decimal RSSI { get; set; }


        ///<summary>
        /// 采样模批号
        ///</summary>
        [MaxLength(256)]
        [Display(Name = "采样模批号", Order = 48)]
        public virtual string BatchNumber { get; set; }


        ///<summary>
        /// 创建时间
        ///</summary>
        [Display(Name = "创建时间", Order = 49)]
        public virtual DateTime CreationTime { get; set; }


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