using DataPlatformTrans.TransBLL;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using DataPlatformTrans.DataEntitys;
using System.Net;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace HDDataTransTester
{
    class Program
    {
        static DataTransDataProvider _DataTransDataProvider = null;
        static void Main(string[] args)
        {
            _DataTransDataProvider = new DataTransDataProvider();
            string sErr;
            if(!_DataTransDataProvider.Start(out sErr))
            {
                Console.WriteLine(sErr);
                Console.WriteLine("程序启动失败。");
                return;
            }
            ProUserCmd();
        }
        static void ProUserCmd()
        {
            while (true)
            {
                ConsoleKeyInfo keyinfo = Console.ReadKey();
                if (keyinfo.Key == ConsoleKey.F2)
                {
                    //不要显示日志了
                    _DataTransDataProvider._Logger.IsShowLog = !_DataTransDataProvider._Logger.IsShowLog;
                }
                else if (keyinfo.Key == ConsoleKey.F3)
                {
                    //不要显示错误了
                    _DataTransDataProvider._Logger.IsShowErr = !_DataTransDataProvider._Logger.IsShowErr;
                }
                else if (keyinfo.Key == ConsoleKey.F4)
                {
                    #region 显示任务
                    Console.WriteLine(_DataTransDataProvider.TransInformation_GetTaskDetail());
                    #endregion
                }
                else if (keyinfo.Key == ConsoleKey.F5)
                {
                    #region 显示任务
                    if (AppConfig.Stations != null && AppConfig.Stations.Count > 0)
                    {
                        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(AppConfig.Stations));
                    }
                    else
                    {
                        Console.WriteLine("无站点信息！");
                    }
                    #endregion
                }
                else
                {
                    string strExit = Console.ReadLine();
                    if (string.Compare("exit", strExit, true) == 0)
                    {
                        return;
                    }
                }
            }
        }
    }
}
