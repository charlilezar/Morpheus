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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Application.Dtos;

using HengDa.LiZongMing.REAMS;
namespace HengDa.LiZongMing.REAMS.HVE.Dtos
{

    /// <summary>
    /// 高压电离伽马记录 Dto
    /// </summary>
    public class HveRecordPagedRequest : PagedAndSortedResultRequestDto
    {
        #region 不保留 基本属性

        ///<summary>
        /// 查询开始时间
        ///</summary>
        [Display(Name = "查询开始时间", Order = 10)]
        public virtual DateTime? TimeStart { get; set; }

        ///<summary>
        /// 查询结束时间
        ///</summary>
        [Display(Name = "查询结束时间", Order = 10)]
        public virtual DateTime? TimeEnd { get; set; }
                 
        ///<summary>
        /// 设备Id
        ///</summary>
        [Display(Name = "设备Id", Order = 19)]
        public virtual long? DeviceId { get; set; }


        #endregion

        #region 保留  手动添加属性

        #endregion

        /// <summary>
        /// 正常化排序使用
        /// </summary>
        public void Normalize()
        {
            //if (string.IsNullOrEmpty(Sorting))
            //{
            //    Sorting = "Id";
            //}
        }
    }
}