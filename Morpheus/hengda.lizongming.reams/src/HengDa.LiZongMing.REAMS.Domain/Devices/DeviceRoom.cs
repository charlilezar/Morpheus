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
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;


namespace HengDa.LiZongMing.REAMS.Devices
{

    /// <summary>
    /// 站房信息
    /// </summary>

    [Table(REAMSDbProperties.DbTablePrefix + "DeviceRoom")]
    public partial class DeviceRoom : FullAuditedEntity<long>, IMultiTenant  // Entity<long> , IHasCreationTime, IHasModificationTime, IExtendableObject, IMustHaveTenant, IContent, IReadPiont, ISeo
    {
        #region 构造方法
        public DeviceRoom()
        {
        }
        #endregion


        #region 基本属性

        // ///<summary>
        // /// 编号
        // ///</summary>
        // [Key]
        // [Column(Order = 1)]
        // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        // [Required()]
        // [Display(Name = "编号", Order = 1)]
        // [UIHint("NotEdit")]
        // public override long Id { get; set; }

        ///<summary>
        /// 名称
        ///</summary>
        [MaxLength(256)]
        [Display(Name = "名称", Order = 2)]
        public virtual string Name { get; set; }


        ///<summary>
        /// 简称
        ///</summary>
        [MaxLength(256)]
        [Display(Name = "简称", Order = 3)]
        public virtual string ShortName { get; set; }


        ///<summary>
        /// 客户Id
        ///</summary>
        [Display(Name = "客户Id", Order = 4)]
        public virtual Guid CustomerId { get; set; }


        ///<summary>
        /// 通讯用户名
        ///</summary>
        [MaxLength(256)]
        [Display(Name = "通讯用户名", Order = 5)]
        public virtual string MqttUserName { get; set; }


        ///<summary>
        /// 通讯密码
        ///</summary>
        [MaxLength(256)]
        [Display(Name = "通讯密码", Order = 6)]
        public virtual string MqttPassword { get; set; }


        ///<summary>
        /// 序列号
        ///</summary>
        [MaxLength(256)]
        [Display(Name = "序列号", Order = 7)]
        public virtual string SN { get; set; }


        ///<summary>
        /// 网卡MAC
        ///</summary>
        [MaxLength(256)]
        [Display(Name = "网卡MAC", Order = 8)]
        public virtual string Mac { get; set; }


        ///<summary>
        /// 设备型号
        ///</summary>
        [MaxLength(256)]
        [Display(Name = "设备型号", Order = 9)]
        public virtual string ProductModel { get; set; }


        ///<summary>
        /// 出厂日期
        ///</summary>
        [Display(Name = "出厂日期", Order = 10)]
        public virtual DateTime ManufactureDate { get; set; }


        ///<summary>
        /// 安装日期
        ///</summary>
        [Display(Name = "安装日期", Order = 11)]
        public virtual DateTime SetupDate { get; set; }


        ///<summary>
        /// 地理位置-经度
        ///</summary>
        [Display(Name = "地理位置-经度", Order = 12)]
        public virtual decimal GeoLocation_Lng { get; set; }


        ///<summary>
        /// 地理位置-伟度
        ///</summary>
        [Display(Name = "地理位置-伟度", Order = 13)]
        public virtual decimal GeoLocation_Lat { get; set; }


        ///<summary>
        /// 地理位置-海拔高度
        ///</summary>
        [Display(Name = "地理位置-海拔高度", Order = 14)]
        public virtual decimal GeoLocation_Altitude { get; set; }


        ///<summary>
        /// 部署地址
        ///</summary>
        [MaxLength(256)]
        [Display(Name = "部署地址", Order = 15)]
        public virtual string TestAddress { get; set; }


        ///<summary>
        /// 最后保养日期
        ///</summary>
        [Display(Name = "最后保养日期", Order = 16)]
        public virtual DateTime LastMaintainDate { get; set; }


        ///<summary>
        /// 下次保养日期
        ///</summary>
        [Display(Name = "下次保养日期", Order = 17)]
        public virtual DateTime NextMaintainDate { get; set; }


        ///<summary>
        /// 硬件连接参数
        ///</summary>
        [MaxLength(2048)]
        [Display(Name = "硬件连接参数", Order = 18)]
        public virtual string HWConnectionJson { get; set; }


        ///<summary>
        /// 有效状态 
        ///</summary>
        [Display(Name = "有效状态 ", Order = 19)]
        public virtual bool IsEnable { get; set; }


        ///<summary>
        /// 备注
        ///</summary>
        [MaxLength(2048)]
        [Display(Name = "备注", Order = 20)]
        public virtual string Remark { get; set; }


        ///<summary>
        /// 最后上线时间
        ///</summary>
        [Display(Name = "最后上线时间", Order = 21)]
        public virtual DateTime LastTimeOnline { get; set; }


        ///<summary>
        /// 最后掉线时间
        ///</summary>
        [Display(Name = "最后掉线时间", Order = 22)]
        public virtual DateTime LastTimeOffline { get; set; }


        ///<summary>
        /// 多租户
        ///</summary>
        [Display(Name = "多租户", Order = 23)]
        public virtual Guid? TenantId { get; set; }


        #endregion

        #region 保留  手动添加属性

        #endregion

    }
}