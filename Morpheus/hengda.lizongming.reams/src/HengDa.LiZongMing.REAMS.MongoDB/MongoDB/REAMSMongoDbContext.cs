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
    [ConnectionStringName(REAMSDbProperties.ConnectionStringName)]
    public partial class REAMSMongoDbContext : AbpMongoDbContext, IREAMSMongoDbContext 
    {


        #region 基本

        #endregion



        #region OnModelCreating
        protected override void CreateModel(IMongoModelBuilder builder)
        {

            base.CreateModel(builder);

            #region 保留  共用的用户属性CreateModel
            builder.Entity<HengDa.LiZongMing.REAMS.Users.AppUser>(b =>
            {
                /* Sharing the same "AbpUsers" collection
                 * with the Identity module's IdentityUser class. */
                b.CollectionName = Volo.Abp.Identity.AbpIdentityDbProperties.DbTablePrefix + "Users";
            });
            #endregion


            /* Configure your own tables/entities inside the  method */

            builder.ConfigureREAMS();

        }

        #endregion

    }

}
