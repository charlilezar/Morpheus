using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.ComponentModel;

namespace HengDa.LiZongMing.Yun.Devices.Dtos
{
    /// <summary>
    /// 解析Enum返回Name，Value阵列
    /// 注：在SL中，有些方法不存在
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <example>
    ///         //==========使用方法============
    ///  public enum TestEnum
    ///  {
    ///      [Description("没有")]
    ///      None = 0,
    ///     [Description("已经有了")]
    ///     Existing = 1,
    ///     [Description("新的")]
    ///     NewOpportunity = 2,
    /// }
    ///  private static void Test()
    /// {
    ///    comboBox1.DisplayMemberPath="Name"
    ///    comboBox1.SelectedValuePath="Value"
    ///    //comboBox1.DataSource = Enum.GetValues(typeof(TestEnum));
    ///    comboBox1.DataSource = EnumNameValue<TestEnum>.ParseEnum();
    ///    comboBox1.SelectedItem = TestEnum.Existing; 
    /// }
    ///  //===================
    /// </example>

    public class EnumNameValue
    {

        


    }


}
