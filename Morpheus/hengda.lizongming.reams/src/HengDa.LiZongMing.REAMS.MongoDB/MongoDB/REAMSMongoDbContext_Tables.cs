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
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;
using MongoDB.Driver;
//using HengDa.LiZongMing.REAMS.Users;

using HengDa.LiZongMing.REAMS;
using HengDa.LiZongMing.REAMS.RNS;
using HengDa.LiZongMing.REAMS.Room;
using HengDa.LiZongMing.REAMS.NBS;
using HengDa.LiZongMing.REAMS.NaI;
using HengDa.LiZongMing.REAMS.HVE;
using HengDa.LiZongMing.REAMS.ZJC;
using HengDa.LiZongMing.REAMS.Devices;
using HengDa.LiZongMing.REAMS.DLL;
using HengDa.LiZongMing.REAMS.Atm;
using HengDa.LiZongMing.REAMS.ZCQ;
using HengDa.LiZongMing.REAMS.Crm;


namespace HengDa.LiZongMing.REAMS.MongoDB
{



    /// <summary>
    /// 
    /// </summary>
    //[ConnectionStringName(REAMSDbProperties.ConnectionStringName)]
    public partial class REAMSMongoDbContext : AbpMongoDbContext, IREAMSMongoDbContext 
    {
        #region 保留  共用的用户属性
        /// <summary>
        /// Users
        /// </summary>
        public IMongoCollection<HengDa.LiZongMing.REAMS.Users.AppUser> Users => Collection<HengDa.LiZongMing.REAMS.Users.AppUser>();
        #endregion


        #region 实体

        /// <summary>
        /// 气象数据
        /// </summary>
        public IMongoCollection<AtmosphereRecord> AtmosphereRecord => Collection<AtmosphereRecord>();

        /// <summary>
        /// 客户信息
        /// </summary>
        public IMongoCollection<Customer> Customer => Collection<Customer>();

        /// <summary>
        /// 工程师信息
        /// </summary>
        public IMongoCollection<CustomerService> CustomerService => Collection<CustomerService>();

        /// <summary>
        /// 设备信息
        /// </summary>
        public IMongoCollection<Device> Device => Collection<Device>();

        /// <summary>
        /// 站房信息
        /// </summary>
        public IMongoCollection<DeviceRoom> DeviceRoom => Collection<DeviceRoom>();

        /// <summary>
        /// 设备报警记录
        /// </summary>
        public IMongoCollection<IotDeviceAlarm> IotDeviceAlarm => Collection<IotDeviceAlarm>();

        /// <summary>
        /// 设备日志
        /// </summary>
        public IMongoCollection<IotDeviceLog> IotDeviceLog => Collection<IotDeviceLog>();

        /// <summary>
        /// DLL检测记录
        /// </summary>
        public IMongoCollection<DllRecord> DllRecord => Collection<DllRecord>();

        /// <summary>
        /// DLL设备参数信息
        /// </summary>
        public IMongoCollection<DllSetting> DllSetting => Collection<DllSetting>();

        /// <summary>
        /// 高压电离记录
        /// </summary>
        public IMongoCollection<HveExtRecord> HveExtRecord => Collection<HveExtRecord>();

        /// <summary>
        /// 高压电离伽马记录
        /// </summary>
        public IMongoCollection<HveRecord> HveRecord => Collection<HveRecord>();

        /// <summary>
        /// 高压电离状态
        /// </summary>
        public IMongoCollection<HveRunStatus> HveRunStatus => Collection<HveRunStatus>();

        /// <summary>
        /// 碘化纳记录
        /// </summary>
        public IMongoCollection<NaiRecord> NaiRecord => Collection<NaiRecord>();

        /// <summary>
        /// 超大气溶胶采样记录
        /// </summary>
        public IMongoCollection<NbsRecord> NbsRecord => Collection<NbsRecord>();

        /// <summary>
        /// 超大气溶胶运行状态
        /// </summary>
        public IMongoCollection<NbsRunStatus> NbsRunStatus => Collection<NbsRunStatus>();

        /// <summary>
        /// 降雨状态
        /// </summary>
        public IMongoCollection<RnsRunStatus> RnsRunStatus => Collection<RnsRunStatus>();

        /// <summary>
        /// 站房环境状态
        /// </summary>
        public IMongoCollection<RoomRunStatus> RoomRunStatus => Collection<RoomRunStatus>();

        /// <summary>
        /// 气碘采样记录
        /// </summary>
        public IMongoCollection<ZcqRecord> ZcqRecord => Collection<ZcqRecord>();

        /// <summary>
        /// 气碘运行状态
        /// </summary>
        public IMongoCollection<ZcqRunStatus> ZcqRunStatus => Collection<ZcqRunStatus>();

        /// <summary>
        /// 湿沉降降雨记录
        /// </summary>
        public IMongoCollection<ZjcRainRecord> ZjcRainRecord => Collection<ZjcRainRecord>();

        /// <summary>
        /// 干沉降记录
        /// </summary>
        public IMongoCollection<ZjcRecord> ZjcRecord => Collection<ZjcRecord>();

        /// <summary>
        /// 干湿沉降实时数据
        /// </summary>
        public IMongoCollection<ZjcRunStatus> ZjcRunStatus => Collection<ZjcRunStatus>();

        #endregion

    }

}
