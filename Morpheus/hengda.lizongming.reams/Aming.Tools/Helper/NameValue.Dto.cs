
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//using Volo.Abp.Application.Dtos;

using HengDa.LiZongMing.Yun;
namespace Aming.Core
{

    /// <summary>
    /// NameValue
    /// </summary>
    //[AutoMapFrom(typeof(DeviceTest))]
    public partial class NameValue<T>
    {
      

        #region 基本属性



        public virtual string Name { get; set; }


        public virtual T Value { get; set; }




        #endregion

     


    }
}