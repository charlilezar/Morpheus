using RSNetDevice;
using RSNetDevice.Data;
using RSNetDevice.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RSDeviceDemo
{
    public partial class Form1 : Form
    {
        private RSServer rsServer;//定义监听服务对象
        public Form1()
        {
            InitializeComponent();
        }

        #region 窗体事件处理

        private void btnStart_Click(object sender, EventArgs e)//启动按钮
        {            

            rsServer = RSServer.Initiate(txtHost.Text, int.Parse(txtPort.Text));//初始化
            rsServer.OnReceiveLoginData += RsServer_OnReceiveLoginData;//登录帧数据接收处理
            rsServer.OnReceiveRealtimeData += RsServer_OnReceiveRealtimeData;//实时数据接收处理
            rsServer.OnReceiveStoreData += RsServer_OnReceiveStoreData;//存储数据接收处理
            rsServer.OnTelecontrolAck += RsServer_OnTelecontrolAck;//遥控指令应答处理
            rsServer.OnTimmingAck += RsServer_OnTimmingAck;//校时命令应答处理
            rsServer.OnReceiveParamIds += RsServer_OnReceiveParamIds;//接收设备参数列表处理
            rsServer.OnReceiveParam += RsServer_OnReceiveParam;//接收设备参数处理
            rsServer.OnReceiveWriteParamAck += RsServer_OnReceiveWriteParamAck;//下载参数指令应答处理
            rsServer.OnReceiveTransDataAck += RsServer_OnReceiveTransDataAck;//透传指令应答处理
            bool res = rsServer.Start();//启动监听服务
            btnStop.Enabled = !(btnStart.Enabled = !res);//设置停启按钮的状态
        }

    

        private void btnStop_Click(object sender, EventArgs e) //停止按钮
        {
            rsServer.OnReceiveLoginData -= RsServer_OnReceiveLoginData;//登录帧数据接收处理
            rsServer.OnReceiveRealtimeData -= RsServer_OnReceiveRealtimeData;//取消注册，防止再次启动时多次触发
            rsServer.OnReceiveStoreData -= RsServer_OnReceiveStoreData;//取消注册
            rsServer.OnTelecontrolAck -= RsServer_OnTelecontrolAck;//取消注册
            rsServer.OnTimmingAck -= RsServer_OnTimmingAck;//取消注册
            rsServer.OnReceiveParamIds -= RsServer_OnReceiveParamIds;//取消注册
            rsServer.OnReceiveParam -= RsServer_OnReceiveParam;//取消注册
            rsServer.OnReceiveWriteParamAck -= RsServer_OnReceiveWriteParamAck;//取消注册
            rsServer.OnReceiveTransDataAck -= RsServer_OnReceiveTransDataAck;//取消注册
            bool res = rsServer.Stop();//停止监听服务
            btnStop.Enabled = !(btnStart.Enabled = res);//设置停启按钮的状态
        }
        private void chk_Click(object sender, EventArgs e)//遥控指令
        {
            CheckBox chk = sender as CheckBox;
            int relayid = Convert.ToInt32(chk.Tag);
            rsServer.Telecontrol(Convert.ToInt32(txtDeviceID.Text), relayid, chk.Checked ? 0 : 1);  //发送遥控指令
        }
        private void btnTimming_Click(object sender, EventArgs e)
        {
            rsServer.Timming(Convert.ToInt32(txtDeviceID.Text));//发送校时指令
        }
        private void btnCallStore_Click(object sender, EventArgs e)
        {
            rsServer.CallStoreData(Convert.ToInt32(txtDeviceID.Text));//发送召唤数据指令
        }
        private void btnReadParametersList_Click(object sender, EventArgs e) //召唤参数编号列表
        {
            if (btnStart.Enabled)
            {
                MessageBox.Show("服务未启动");
                return;
            }
            rsServer.CallParamList(Convert.ToInt32(txtDeviceID.Text));//发送召唤设备参数列表指令
        }
        private void btnReadParameters_Click(object sender, EventArgs e)//读取设备参数
        {
            if (btnStart.Enabled)
            {
                MessageBox.Show("服务未启动");
                return;
            }
            List<int> ids = new List<int>();
            string[] idArray = txtParamIds.Text.Split(',');
            foreach (string str in idArray)
            {
                try
                {
                    ids.Add(Convert.ToInt32(str));
                }
                catch
                { }
            }
            if (ids.Count >= 115)
            {
                MessageBox.Show("一次读取参数数量不能超过115个");
                return;
            }
            rsServer.CallParam(Convert.ToInt32(txtDeviceID.Text), ids);//读取设备参数
        }
        private void btnWriteParameters_Click(object sender, EventArgs e)//下载设备参数
        {
            if (btnStart.Enabled)
            {
                MessageBox.Show("服务未启动");
                return;
            }


            List<ParamItem> parameters = new List<ParamItem>();

            try
            {
                
                parameters.Add(ParamItem.New(Convert.ToInt32(txtParamId.Text), txtParamVal.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            if (parameters.Count > 115)
            {
                MessageBox.Show("一次性下发参数数量不能超过115个");
                return;
            }
            rsServer.WriteParam(Convert.ToInt32(txtDeviceID.Text), parameters);
        }

        private void btnTrans_Click(object sender, EventArgs e)//数据透传
        {
            if (btnStart.Enabled)
            {
                MessageBox.Show("服务未启动");
                return;
            }

            if (!IsHexadecimal(txtTransData.Text))
            {
                MessageBox.Show("数据格式错误，请输入正确的16进制字符串");
                return;
            }
            rsServer.Trans(Convert.ToInt32(txtDeviceID.Text), txtTransData.Text);
        }
        #endregion
        #region rsServer事件处理

        private void RsServer_OnReceiveLoginData(RSServer server, LoginData data)
        {
            //设备登录指令处理
            Invoke(new Action(() =>
            {
                rhtResult.AppendText("设备登录->设备编号：" + data.DeviceID + "\r\n");
            }));
        }
        private void RsServer_OnTimmingAck(RSServer server, TimmingAck ack)
        { //校时指令应答处理
            Invoke(new Action(() =>
            {
                rhtResult.AppendText("校时应答->设备编号：" + ack.DeviceID + "\t执行结果：" + ack.ExecStatus + "\r\n");
            }));
        }

        private void RsServer_OnTelecontrolAck(RSServer server, TelecontrolAck ack)
        {//遥控指令应答处理
            Invoke(new Action(() =>
            {
                rhtResult.AppendText("遥控应答->设备编号：" + ack.DeviceID + "\t继电器编号：" + ack.RelayID + "\t执行结果：" + ack.ExecStatus + "\r\n");
            }));
        }

        private void RsServer_OnReceiveStoreData(RSServer server, StoreData data)
        { //存储数据接收处理
            foreach (NodeData ndata in data.NodeList)//遍历节点数据。数据包括网络设备的数据以及各个节点数据。温湿度数据存放在节点数据中
            {
                string str = "存储数据->设备编号：" + data.DeviceID + "\t节点编号：" + ndata.NodeID + "\t温度：" + ndata.Tem + "\t湿度：" + ndata.Hum + "\t时间：" + ndata.RecordTime.ToString("yyyy-MM-dd HH:mm:ss") + "\t坐标类型：" + ndata.CoordinateType.ToString() + "\t经度：" + ndata.Lng + "\t维度：" + ndata.Lat;
                Invoke(new Action(() =>
                {
                    rhtResult.AppendText(str + "\r\n");
                }));

            }
        }

        private void RsServer_OnReceiveRealtimeData(RSServer server, RealTimeData data)
        { //实时数据接收处理
            foreach (NodeData ndata in data.NodeList)//遍历节点数据。数据包括网络设备的数据以及各个节点数据。温湿度数据存放在节点数据中
            {
                string str = "实时数据->设备编号：" + data.DeviceID + "\t经度：" + data.Lng + "\t纬度：" + data.Lat + "\t坐标类型：" + data.CoordinateType.ToString() + "\t节点编号：" + ndata.NodeID +
                    "\t温度：" + ndata.Tem +
                    "\t湿度：" + ndata.Hum +
                    "\t继电器状态：" + data.RelayStatus +
            "\t浮点型数据：" + ndata.FloatValue +
            "\t32位有符号数据：" + ndata.SignedInt32Value +
            "\t32位无符号数据：" + ndata.UnSignedInt32Value;
                Invoke(new Action(() =>
                {
                    rhtResult.AppendText(str + "\r\n");
                }));

            }
        }

        private void RsServer_OnReceiveParamIds(RSServer server, ParamIdsData data)
        { //设备参数列表接收处理            

            string str = "设备参数编号列表->设备编号：" + data.DeviceID + "\t参数总数量：" + data.TotalCount + "\t本帧参数数量：" + data.Count + "\r\n";
            foreach (int paramId in data.PararmIdList)//遍历设备中参数id编号
            {
                str += paramId + ",";
            }
            Invoke(new Action(() =>
            {
                rhtResult.AppendText(str + "\r\n");
            }));

        }

        private void RsServer_OnReceiveParam(RSServer server, ParamData data)
        {//设备参数接收处理

            string str = "设备参数->设备编号：" + data.DeviceID + "\r\n";

            foreach (ParamItem pararm in data.ParameterList)
            {
                str += "参数编号：" + pararm.ParamId + "\t参数描述：" + pararm.Description + "\t参数值：" + (pararm.ValueDescription == null ? pararm.Value : pararm.ValueDescription[pararm.Value]) + "\r\n";
            }
            Invoke(new Action(() =>
            {
                rhtResult.AppendText(str + "\r\n");
            }));
        }
        private void RsServer_OnReceiveWriteParamAck(RSServer server, WriteParamAck data)
        {//下载设备参数指令应答处理

            string str = "下载设备参数->设备编号：" + data.DeviceID + "\t参数数量：" + data.Count + "\t" + (data.Res ? "下载成功" : "下载失败");
            Invoke(new Action(() =>
            {
                rhtResult.AppendText(str + "\r\n");
            }));
        }

        private void RsServer_OnReceiveTransDataAck(RSServer server, TransDataAck data)
        {//透传指令应答处理

            string str = "数据透传->设备编号：" + data.DeviceID + "\t响应结果：" + data.Data + "\r\n字节数：" + data.TransDataLen;
            Invoke(new Action(() =>
            {
                rhtResult.AppendText(str + "\r\n");
            }));

        }

        #endregion
        #region 私有方法
        /// <summary>
        /// 判断是否为合法的16进制字符串
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public bool IsHexadecimal(string hexString)
        {
            try
            {
                hexString = hexString.Replace(" ", "").Replace("\r", "").Replace("\n", "");
                if ((hexString.Length % 2) != 0)
                    hexString += " ";
                byte[] returnBytes = new byte[hexString.Length / 2];
                for (int i = 0; i < returnBytes.Length; i++)
                    returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
