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
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;


using HengDa.LiZongMing.REAMS.NBS;
using HengDa.LiZongMing.REAMS.NBS.Dtos;

namespace HengDa.LiZongMing.REAMS.NBS
{

    /// <summary>
    /// 超大气溶胶运行状态 EditDto
    /// </summary>

    public partial interface INbsRunStatusAppService : ICrudAppService<NbsRunStatusDto, Guid, NbsRunStatusPagedRequest, NbsRunStatusDto, NbsRunStatusDto>
    {
         #region // 自己的方法加这里或是用分部类

        /// <summary>
        ///  获取一个现有实体，或者新的默认实体
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //Task<NbsRunStatusDto> GetOrNew(Guid? input);

        #endregion


    }
}