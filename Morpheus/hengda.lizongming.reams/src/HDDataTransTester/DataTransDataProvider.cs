using DataPlatformTrans.DataEntitys;
using DataPlatformTrans.DataEntitys.MessageEntity;
using DataPlatformTrans.DataFactory;
using DataPlatformTrans.TransBLL;
using log4net.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HDDataTransTester.RequestData;
using HDDataTransTester.RequestData.RequestDataEntitys;

namespace HDDataTransTester
{
    public class DataTransDataProvider
    {
        DataPlatformTransManager _DataPlatformTransManager = null;
        public DataPlatformTrans.DataEntitys.MessageEntity.AppLogger  _Logger = null;
        /// <summary>
        /// 数据中间件，为上传平台提供数据来源，所有数据都在当前类中实现，方法可以是接口或者是从查询数据库，也可以定位2者都可以，这个就在该类中去处理
        /// </summary>
        DataPlatformTrans.DataFactory.CPDataFactory _CPDataFactory = null;
        public DataTransDataProvider()
        {

        }
        public bool Start(out string sErr)
        {
            _Logger = new DataPlatformTrans.DataEntitys.MessageEntity.AppLogger();
            _Logger.ShowLogAsynNotice += AppLogger_ShowLogAsynNotice;
            _Logger.ShowErrAsynNotice += AppLogger_ShowErrAsynNotice;
            _Logger.Logger = new LoggerFactory().AddLog4Net().CreateLogger("logs");
            TransConfig config = new TransConfig()
            {
                IPEndPoint = new System.Net.IPEndPoint(IPAddress.Parse("127.0.0.1"), 502),
                TimeOut = 3000,//定义连接超时
                ReceivedTimeOut = 2000
            };
            this._CPDataFactory = new DataPlatformTrans.DataFactory.CPDataFactory();
            this.InitDataFactoryFromWebApi(this._CPDataFactory);
            _DataPlatformTransManager = new DataPlatformTransManager(config, this._CPDataFactory, _Logger);
            if (!DataPlatformTrans.DataEntitys.AppConfig.ReadConfigFromJsonFile("Stations.json", out sErr))
            {
                _Logger.ShowErr(this, sErr);
            }
            else
            {
                string strStations = string.Empty;
                if (AppConfig.Stations != null && AppConfig.Stations.Count > 0)
                {
                    foreach (AppConfig.StationEntity station in AppConfig.Stations)
                    {
                        strStations += station.SNO + "<Enabled=" + station.Enabled + ">;";
                    }
                }
                _Logger.ShowLog(this, $"已配置站点[{strStations}]。");
            }
            if (!_DataPlatformTransManager.StartListenning(out sErr))
            {
                this._Logger.ShowErr(this, sErr);
                return false;
            }
            return true;
        }
        #region 消息及日志管理
        /// <summary>
        /// 显示任务情况
        /// </summary>
        /// <returns>任务情况的字符窜</returns>
        public string TransInformation_GetTaskDetail()
        {
            if (this._DataPlatformTransManager == null) return "None";
            if (this._DataPlatformTransManager._TaskManager == null) return "TaskManager is null";
            if (this._DataPlatformTransManager._TaskManager.TaskDatas == null ||
                this._DataPlatformTransManager._TaskManager.TaskDatas.Count == 0)
            {
                return "无任务";
            }
            StringBuilder sb = new StringBuilder();
            int iIndex = 1;
            foreach (TransCmdController task in this._DataPlatformTransManager._TaskManager.TaskDatas.Values)
            {
                sb.Append(string.Format("{0}、类型={1},状态={2},QN={3},CN={4},创建时间={5}\r\n"
                    , iIndex++, task.TransCmdType, task.TaskStatu, task.QN, task.CN, task.CreatTime.ToString("yyyy-MM-dd HH:mm:ss")));
            }
            return "当前任务信息如下：\r\n" + sb.ToString();
        }
        private void AppLogger_ShowErrAsynNotice(string sMsg)
        {
            Console.WriteLine("错误消息：->" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "->" + sMsg);
        }
        private void AppLogger_ShowLogAsynNotice(string sMsg)
        {
            Console.WriteLine("日志->" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "->" + sMsg);
        }
        #endregion
        /// <summary>
        /// 通过webapi接口读取数据
        /// </summary>
        private void InitDataFactoryFromWebApi(DataPlatformTrans.DataFactory.CPDataFactory cPDataFactory)
        {
            if(cPDataFactory==null)
            {
                _Logger.ShowErr(this, "初始化WebApi接口时，传入的Factory对象为空！");
                return;
            }
            //气碘实时数据
            cPDataFactory.ZCQRealRecord_GetData = this.WebAPiFactory_GetRealZCQ;
            //高压电离室数据
            cPDataFactory.HveRealRecord_GetData = this.WebAPiFactory_GetRealHve;
            //气象数据
            cPDataFactory.AtmosphereRealRecord_GetData = this.WebAPiFactory_GetRealAtmosphere;
            //超大容量数据
            cPDataFactory.NBSRecord_GetData = this.WebAPiFactory_GetRealNBS;
            //TODO:碘化钠溥仪格式还不知道什么；
            //TODO：干湿沉降字段不是实时记录，这个是降雨记录，无法周期性上传
        }
        /// <summary>
        /// 通过数据库直接访问读取数据
        /// </summary>
        private void InitDataFactoryFromDBDAL()
        {

        }
        #region  传入中间件的功能函数(WEBAPI)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="device">设备信息，包含关键的站点号与ENo等其他信息</param>
        /// <param name="iCTime">数据读取周期，单位：秒，传入这个的目的是上传这个周期时间段内的最新记录</param>
        /// <returns></returns>
        private async Task<RtnMessage<RecordRealZCQ>> WebAPiFactory_GetRealZCQ(DeviceInfo device,int iCTime)
        {
            RequestDataFilter filter = new RequestDataFilter(device.ID, DateTime.Now.AddSeconds(-1 * iCTime));
            return await RequestData.RequestByWebApi.DeviceRequestFuns.ZCQRequestFuns.GetRealAsync(filter);
        }
        /// <summary>
        /// 读取高压电离室记录数据
        /// </summary>
        /// <param name="device"></param>
        /// <param name="iCTime"></param>
        /// <returns></returns>
        private async Task<RtnMessage<RecordRealHve>> WebAPiFactory_GetRealHve(DeviceInfo device, int iCTime)
        {
            RequestDataFilter filter = new RequestDataFilter(device.ID, DateTime.Now.AddSeconds(-1 * iCTime));
            return await RequestData.RequestByWebApi.DeviceRequestFuns.HveRequestFuns.GetRealAsync(filter);
        }
        private async Task<RtnMessage<RecordRealAtmosphere>> WebAPiFactory_GetRealAtmosphere(DeviceInfo device, int iCTime)
        {
            RequestDataFilter filter = new RequestDataFilter(device.ID, DateTime.Now.AddSeconds(-1 * iCTime));
            //读取气象数据
            RtnMessage<RecordRealAtmosphere> rtn = await RequestData.RequestByWebApi.DeviceRequestFuns.AtmosphereRequestFuns.GetRealAsync(filter);
            //读取站房数据，获取感雨器信息
            //TODO:从站房接口中读取感雨器数据
            return rtn;
        }
        private async Task<RtnMessage<RecordRealNBS>> WebAPiFactory_GetRealNBS(DeviceInfo device, int iCTime)
        {
            RequestDataFilter filter = new RequestDataFilter(device.ID, DateTime.Now.AddSeconds(-1 * iCTime));
            return await RequestData.RequestByWebApi.DeviceRequestFuns.NBSRequestFuns.GetRealAsync(filter);
        }
        #endregion
    }
}
