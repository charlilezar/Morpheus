using System.Collections.Concurrent;
//using Microsoft.Extensions.Configuration;
//using Abp.Extensions;
//using Aming.AbpCore.Common.Helper;
//using Abp.Reflection.Extensions;
using System;
using System.Reflection;

namespace Aming.AbpCore.Common.Helper
{
    /// <summary>
    /// 访问私有对象
    /// </summary>
    public class AccessPrivateFieldHelp
    {
        public static object GetPrivateField(object instance, string name)
        {
            object result = null;
            try
            {
                Type t = instance.GetType();
                result = t.InvokeMember(name, BindingFlags.Instance |
                    BindingFlags.NonPublic | BindingFlags.GetField, null,
                    instance, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public static void SetPrivateFiled(object instance, string name,
            object[] parameters)
        {
            try
            {
                Type t = instance.GetType();
                t.InvokeMember(name, BindingFlags.Instance |
                    BindingFlags.NonPublic | BindingFlags.SetField, null,
                    instance, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static object ExecutePrivateMethod(object instance, string name,
            object[] parameters)
        {
            object result = null;
            try
            {
                Type t = instance.GetType();
                result = t.InvokeMember(name, BindingFlags.Instance |
                    BindingFlags.NonPublic | BindingFlags.InvokeMethod, null,
                    instance, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}


