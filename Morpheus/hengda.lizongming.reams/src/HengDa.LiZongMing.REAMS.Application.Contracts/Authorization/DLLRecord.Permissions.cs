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

namespace HengDa.LiZongMing.REAMS.Authorization
{
    public partial class REAMSPermissions
	{
		/// <summary>
		/// 定义系统的权限名称的字符串常量。
		/// <see cref="AuthorizationProvider" />中对权限的定义.
		///</summary>
		public static partial class DllRecord
        {
            /// <summary>
            /// IotDeviceShare权限节点
            ///</summary>
            public const string Default = GroupName + ".DllRecord";


		    /// <summary>
		    /// DllRecord查询授权
		    ///</summary>
		    public const string Query = Default + ".Query";

		    /// <summary>
		    /// DllRecord创建权限
		    ///</summary>
		    public const string Create = Default + ".Create";

		    /// <summary>
		    /// DllRecord修改权限
		    ///</summary>
		    public const string Edit = Default + ".Edit";

		    /// <summary>
		    /// DllRecord删除权限
		    ///</summary>
		    public const string Delete = Default + ".Delete";

            /// <summary>
		    /// DllRecord批量删除权限
		    ///</summary>
		    public const string BatchDelete = Default + ".BatchDelete";

		    /// <summary>
		    /// DllRecord导出Excel
		    ///</summary>
		    public const string ExportExcel= Default + ".ExportExcel";

        }
    }

}


