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
namespace HengDa.LiZongMing.REAMS.Devices.Dtos
{

    /// <summary>
    /// 设备信息 ListDto
    /// </summary>
    //[AutoMapFrom(typeof(Device))]
public partial class DeviceListDto : EntityDto<long>
    {
        #region 构造方法
        public DeviceListDto()
        {
        }
        #endregion


        #region 基本属性

        // ///<summary>
        // /// Id
        // ///</summary>
        // [Display(Name = "Id", Order = 23)]
        // public virtual long Id { get; set; }


        ///<summary>
        /// 站房Id
        ///</summary>
        [Display(Name = "站房Id", Order = 24)]
        public virtual long DeviceRoomId { get; set; }


        ///<summary>
        /// 客户Id
        ///</summary>
        [Display(Name = "客户Id", Order = 25)]
        public virtual Guid CustomerId { get; set; }


        ///<summary>
        /// 设备友好名称
        ///</summary>
        [MaxLength(256)]
        [Display(Name = "设备友好名称", Order = 26)]
        public virtual string Name { get; set; }


        ///<summary>
        /// 通讯用户名
        ///</summary>
        [MaxLength(256)]
        [Display(Name = "通讯用户名", Order = 27)]
        public virtual string MqttUserName { get; set; }


        ///<summary>
        /// 通讯密码
        ///</summary>
        [MaxLength(256)]
        [Display(Name = "通讯密码", Order = 28)]
        public virtual string MqttPassword { get; set; }


        ///<summary>
        /// 设备通讯序列号
        ///</summary>
        [MaxLength(256)]
        [Display(Name = "设备通讯序列号", Order = 29)]
        public virtual string SN { get; set; }


        ///<summary>
        /// 设备名称
        ///</summary>
        [MaxLength(256)]
        [Display(Name = "设备名称", Order = 30)]
        public virtual string ProductName { get; set; }


        ///<summary>
        /// 设备型号
        ///</summary>
        [MaxLength(256)]
        [Display(Name = "设备型号", Order = 31)]
        public virtual string ProductModel { get; set; }


        ///<summary>
        /// 出厂日期
        ///</summary>
        [Display(Name = "出厂日期", Order = 32)]
        public virtual DateTime ManufactureDate { get; set; }


        ///<summary>
        /// 销售代表
        ///</summary>
        [MaxLength(256)]
        [Display(Name = "销售代表", Order = 33)]
        public virtual string Sales { get; set; }


        ///<summary>
        /// 售后客服
        ///</summary>
        [MaxLength(256)]
        [Display(Name = "售后客服", Order = 34)]
        public virtual string CustomerService { get; set; }


        ///<summary>
        /// 地理位置-经度
        ///</summary>
        [Display(Name = "地理位置-经度", Order = 35)]
        public virtual decimal GeoLocation_Lng { get; set; }


        ///<summary>
        /// 地理位置-伟度
        ///</summary>
        [Display(Name = "地理位置-伟度", Order = 36)]
        public virtual decimal GeoLocation_Lat { get; set; }


        ///<summary>
        /// 地理位置-海拔高度
        ///</summary>
        [Display(Name = "地理位置-海拔高度", Order = 37)]
        public virtual decimal GeoLocation_Altitude { get; set; }


        ///<summary>
        /// 部署地址
        ///</summary>
        [MaxLength(256)]
        [Display(Name = "部署地址", Order = 38)]
        public virtual string TestAddress { get; set; }


        ///<summary>
        /// 硬件连接参数
        ///</summary>
        [MaxLength(2048)]
        [Display(Name = "硬件连接参数", Order = 39)]
        public virtual string HWConnectionJson { get; set; }


        ///<summary>
        /// 有效状态 
        ///</summary>
        [Display(Name = "有效状态 ", Order = 40)]
        public virtual bool IsEnable { get; set; }


        ///<summary>
        /// 备注
        ///</summary>
        [MaxLength(2048)]
        [Display(Name = "备注", Order = 41)]
        public virtual string Remark { get; set; }


        ///<summary>
        /// 最后上线时间
        ///</summary>
        [Display(Name = "最后上线时间", Order = 42)]
        public virtual DateTime LastTimeOnline { get; set; }


        ///<summary>
        /// 最后掉线时间
        ///</summary>
        [Display(Name = "最后掉线时间", Order = 43)]
        public virtual DateTime LastTimeOffline { get; set; }


        ///<summary>
        /// 多租户
        ///</summary>
        [Display(Name = "多租户", Order = 44)]
        public virtual Guid? TenantId { get; set; }


        #endregion

        #region 保留  手动添加属性

        #endregion

    }
}