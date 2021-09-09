using DataPlatformTrans.DataEntitys.MessageEntity;
using DataPlatformTrans.DataFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPlatformTrans.DataEntitys.SpecialCmdEntitys
{
    public class CmdToken : CmdData
    {
        public override async Task<RtnMessage> SetCPDataAsync(CPDataFactory factory)
        {
            //身份验证无需传输CP数据
            this.CPData = null;
            return new RtnMessage<string>();
        }
    }
    public class CmdReadData : CmdData
    {
        AppConfig.DeviceEntity.DeviceTypes _DeviceType;
        public CmdReadData(AppConfig.DeviceEntity.DeviceTypes type)
        {
            _DeviceType = type;
            this.CN = (int)TransCNs.实时数据上传;
        }
        public override async Task<RtnMessage> SetCPDataAsync(CPDataFactory factory)
        {
            AppConfig.DeviceEntity device = AppConfig.GetDeviceByDeviceTtype(this.SNo, _DeviceType);
            //关键信息赋值
            if (this.CPData == null)
                this.CPData = new CmdCPData();
            this.CPData.CPDataItems = new List<CmdCPDataItem>();
            CmdCPDataItem item = new CmdCPDataItem();
            item.SNO = this.SNo;
            item.ENo = device.ENO;

            DeviceInfo deviceInfo = new DeviceInfo()
            {
                ID = device.ID,
                SNo = this.SNo,
                ENO = device.ENO,
                MN = this.MN
            };
            int iCtime = device.CTime;
            //获取数据
            SendPlatFormDataBase data = null;
            if (this._DeviceType == AppConfig.DeviceEntity.DeviceTypes.NBS)
            {
                if (factory.NBSRecord_GetData == null)
                {
                    return new RtnMessage("还未实现NBS设备的实时数据读取功能。");
                }
                RtnMessage<RecordRealNBS> rtn = await factory.NBSRecord_GetData(deviceInfo,iCtime);
                if (!rtn.Sucessful)
                {
                    return new RtnMessage(rtn.Msg);
                }
                data = rtn.Result;
            } 
            else if (this._DeviceType == AppConfig.DeviceEntity.DeviceTypes.HVE)
            {
                if (factory.HveRealRecord_GetData == null)
                {
                    return new RtnMessage("还未实现Hve设备的实时数据读取功能。");
                }
                RtnMessage<RecordRealHve> rtn = await factory.HveRealRecord_GetData(deviceInfo, iCtime);
                if (!rtn.Sucessful)
                {
                    return new RtnMessage(rtn.Msg);
                }
                data = rtn.Result;
            }
            else if (this._DeviceType == AppConfig.DeviceEntity.DeviceTypes.ZCQ)
            {
                if (factory.ZCQRealRecord_GetData == null)
                {
                    return new RtnMessage("还未实现ZCQ设备的实时数据读取功能。");
                }
                RtnMessage<RecordRealZCQ> rtn = await factory.ZCQRealRecord_GetData(deviceInfo, iCtime);
                if (!rtn.Sucessful)
                {
                    return new RtnMessage(rtn.Msg);
                }
                data = rtn.Result;
            }
            else if (this._DeviceType == AppConfig.DeviceEntity.DeviceTypes.NaIs)
            {
                if (factory.NaIRealRecord_GetData == null)
                {
                    return new RtnMessage("还未实现NaI设备的实时数据读取功能。");
                }
                RtnMessage<RecordNaI> rtn = await factory.NaIRealRecord_GetData(deviceInfo, iCtime);
                if (!rtn.Sucessful)
                {
                    return new RtnMessage(rtn.Msg);
                }
                data = rtn.Result;
            }
            else if (this._DeviceType == AppConfig.DeviceEntity.DeviceTypes.Atmosphere)
            {
                if (factory.AtmosphereRealRecord_GetData == null)
                {
                    return new RtnMessage("还未实现Atmosphere设备的实时数据读取功能。");
                }
                RtnMessage<RecordRealAtmosphere> rtn = await factory.AtmosphereRealRecord_GetData(deviceInfo, iCtime);
                if (!rtn.Sucessful)
                {
                    return new RtnMessage(rtn.Msg);
                }
                data = rtn.Result;
            }
            else
            {
                return new RtnMessage($"读取实时数据时，传入的设备类型[{_DeviceType}]功能未实现！");
            }
            if (data == null)
            {
                return new RtnMessage($"读取实时数据[{_DeviceType}]时，虽然反馈成功，但返回的数据对象为空！");
            }
            //将data转换成CPDataItem，调用CPDataFormatValue，将数据按照协议内容格式化
            string strColName;
            string strResult;
            string strErr;
            foreach (System.Reflection.PropertyInfo property in data.GetType().GetProperties())
            {
                strColName = property.Name;
                if (strColName.StartsWith("SpCol_"))
                    strColName = strColName.Substring(6);//把前面的特殊标识去掉
                object objValue = property.GetValue(data, null);
                if (!CPDataFormatValue.TryFormatResult(objValue, strColName, out strResult, out strErr))
                {
                    //此时转换失败，就没有必要进行下去了，如果后期平台中心允许跳过错误的字段，那也可以跳过；
                    return new RtnMessage($"命令[{this.CN}]返回值格式化出错：{strErr}");
                }
                item.AddItem(strColName, strResult);
            }
            this.CPData.CPDataItems.Add(item);//实时数据每次上传只传一个设备的，所以只要1个明细记录就可以了
            //TODO:格式化并存入CP
            return new RtnMessage();
        }
    }
}
