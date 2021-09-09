using Aming.DTU;
using Aming.DTU.Config;
using HengDa.LiZongMing.REAMS.Devices.Dtos;

namespace HengDa.LiZongMing.REAMS.CtrlServices
{
    public interface ICtrlService
    {
        DeviceDto Device { get; set; }
        IDataEndPoint MyDataEndPoint { get; set; }
        ITransmissionService TranService { get; set; }

        //void OnUniMessageReceived(IDataEndPoint config, UniMessageBase uniMessageBase);

        /// <summary>
        /// 
        /// </summary>
        event ReceivedUniMessageCallBack OnUniMessageDataReceived;

        event ReceivedUniMessageCallBack OnDataForwardTo;

        void SendUniMessage(UniMessageBase msg);

    }
}