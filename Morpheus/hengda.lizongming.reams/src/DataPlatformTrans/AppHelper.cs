using DataPlatformTrans.DataEntitys;
using DataPlatformTrans.DataEntitys.MessageEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DataPlatformTrans.AppHelper
{
    public class AppFuns
    {
        /// <summary>
        /// 查找指定内容的文本
        /// </summary>
        /// <param name="sData">要查找的文本内容</param>
        /// <param name="sPattern">正则匹配符</param>
        /// <returns>被找到的部分，包含匹配内容</returns>
        public static string FindTextByRegex(string sData, string sPattern)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(sPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match mc = reg.Match(sData);
            if (mc == null)
            {
                return string.Empty;
            }
            if (mc.Value == string.Empty)
            {
                return string.Empty;
            }
            return mc.Value;
        }
        /// <summary>
        /// 在sData中查找与sPattern匹配的第一次的位置
        /// </summary>
        /// <param name="sData">要查找的文本</param>
        /// <param name="sPattern">要查找文本的正则表达式</param>
        /// <param name="iStartIndex">本次查找的起始位置</param>
        /// <returns></returns>
        public static int FindTextFirstIndexByRegex(string sData, string sPattern, int iStartIndex = 0)
        {
            /******
             * 注意：该函数返回的是第一次出现的位置；
             * ********/
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(sPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match mc = reg.Match(sData, iStartIndex);
            if (mc == null || String.IsNullOrEmpty(mc.Value))
            {
                return -1;
            }
            return mc.Index;
        }
        public static string GetCrc16(string sDataSource)
        {
            byte[] bs = Encoding.ASCII.GetBytes(sDataSource);

            //CRC(Modbus)校验码生成
            uint i, j;
            uint crc16 = 0xFFFF;
            for (i = 0; i < bs.Length; i++)
            {
                crc16 ^= bs[i]; // CRC = BYTE xor CRC
                for (j = 0; j < 8; j++)
                {
                    if ((crc16 & 0x01) == 1) //如果CRC最后一位为1右移一位后carry=1则将CRC右移
                                             //一位后再与POLY16=0xA001进行xor运算
                        crc16 = (crc16 >> 1) ^ 0xA001;
                    else //如果CRC最后一位为0则只将CRC右移一位
                        crc16 = crc16 >> 1;
                }
            }
            short iValue = (short)crc16;
            bs = BitConverter.GetBytes(iValue);//转成2个字节的
            return bs[1].ToString("X2") + bs[0].ToString("X2");
        }
    }
    public class TimeStamper
    {
        static string LastTimeStamperValue = string.Empty;
        public static string GetNewTimeStampe()
        {
            //引入该函数的目的是担心两次取值间隔太短不足1毫秒，这也是有肯能的。导致2个任务唯一标识QN一样，这样就
            string str = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            if (str == LastTimeStamperValue)
            {
                Thread.Sleep(1);//休眠1毫秒就够了
                return GetNewTimeStampe();
            }
            else
            {
                LastTimeStamperValue = str;
                return LastTimeStamperValue;
            }
        }
    }
    public class TransCMDDecode
    {
        #region  静态方法
        /// <summary>
        /// 将字符窜转换成命令的数据对象
        /// </summary>
        /// <param name="sData">以ASCII码组成的字符窜</param>
        /// <param name="sErr"></param>
        /// <returns></returns>
        public static CmdData DecodeReceivedDta(string sData, out string sErr)
        {
            /**************
             * 数据解析规则：
             * 1、数据包含2部分，包头、数据段长度、数据段、CRC、包尾
             * 包头：##
             * 数据段长度：固定为4位长度的一个整型字符窜，前面补充0，例如0001，表示长度1位，1001表示长度1001,它表示后面数据的总长度；
             * 数据段：分为命令头部分（cmdHeader）和命令的数据部分(cmdCP)，后者是用CP=&&....&&包含起来；
             * 
             * 
             * *********************/
            if (!sData.StartsWith("##"))
            {
                sErr = $"收到的命令不是以##开头的，无法解析。[{sData}]";
                return null;
            }
            if (sData.Length < 6)
            {
                sErr = $"收到的命令长度不足6位，无法解析。[{sData}]";
                return null;
            }
            string str = sData.Substring(2, 4);
            //解析成整型
            int iLen;
            if (!int.TryParse(str, out iLen) || iLen <= 0)
            {
                sErr = $"收到的命令的长度值不是有效大于0的整型，无法解析。[{sData}]";
                return null;
            }
            if (sData.Length != iLen + 6)
            {
                sErr = $"收到的命令长度不是设定值得{iLen + 6}位，无法解析。[{sData}]";
                return null;
            }
            if (iLen < 6)
            {
                sErr = $"收到的命令长度值位{iLen}，至少无法解析。[{sData}]";
                return null;
            }
            string sDataValid = sData.Substring(6, sData.Length - 6 - 6);//去除头部的##0001部分和结尾4位校验码和\r\n；
            string sCmdHeader, sCmdCP;
            int iCPStart = sDataValid.IndexOf("CP=&&");
            if (iCPStart < 0)
            {
                //此时可以认为没有CP数据，只有命令的头部，目前不允许这样的情况，从协议的示例上看，即便CP内没有内容也会包含CP=&&&&这样的字符
                sErr = $"收到的命令无数据CP部分，至少应包含CP=&&&&，该命令无法解析。[{sData}]";
                return null;
                //sCmdHeader = sDataValid;
                //sCmdCP = string.Empty;
            }
            else
            {
                sCmdHeader = sDataValid.Substring(0, iCPStart);
                sCmdCP = AppHelper.AppFuns.FindTextByRegex(sDataValid, @"CP=&&.*?&&");//这里中间可能会存在空格
                if (sCmdCP.Length < 7)
                {
                    //此时有问题了，按到类有标识符号CP=&&，可定存在数据了，那只能说明没有结尾&&符号，或者不全，这个时候要报错
                    sErr = $"收到的命令无数据CP部分不正确，无法解析。[{sData}]";
                    return null;
                }
                sCmdCP = sCmdCP.Substring(5, sCmdCP.Length - 7);
                //string strPattern = @"CP=&&\S*&&";
            }
            CmdData cmd = new CmdData();
            string[] arr;
            string sName, sValue;
            int iValue;
            #region 头部字符窜sCmdHeader分割
            //开始解析命令头部数据，根据协议规定，各项目都是用分号分割
            arr = sCmdHeader.Split(';');
            foreach (string s in arr)
            {
                if (s.Length == 0) continue;
                if (!DecodeCmd_SplitItem(s, out sName, out sValue, out sErr))
                {
                    sErr = $"收到的命令解析[{s}]时出错：{sErr}，无法解析。[{sData}]";
                    return null;
                }
                sName = sName.ToUpper();
                switch (sName)
                {
                    case "QN":
                        cmd.QN = sValue;
                        break;
                    case "ST":
                        if (!int.TryParse(sValue, out iValue))
                        {
                            sErr = $"收到的命令解析[{sName}]时出错：{sValue}不是有效整型，无法解析。[{sData}]";
                            return null;
                        }
                        cmd.ST = (short)iValue;
                        break;
                    case "CN":
                        if (!int.TryParse(sValue, out iValue))
                        {
                            sErr = $"收到的命令解析[{sName}]时出错：{sValue}不是有效整型，无法解析。[{sData}]";
                            return null;
                        }
                        cmd.CN = (short)iValue;
                        break;
                    case "PW":
                        cmd.PW = sValue;
                        break;
                    case "MN":
                        cmd.MN = sValue;
                        break;
                    case "FLAG":
                        if (!int.TryParse(sValue, out iValue))
                        {
                            sErr = $"收到的命令解析[{sName}]时出错：{sValue}不是有效整型，无法解析。[{sData}]";
                            return null;
                        }
                        cmd.Flag = (short)iValue;
                        break;
                    default:
                        if (sName.Length > 0)
                        {
                            sErr = $"收到的命令解析[{sName}]不是预期的,对应值为{sValue}，无法解析。[{sData}]";
                            return null;
                        }
                        break;
                }
            }
            #endregion
            #region CP字符窜sCmdCP分割
            if (sCmdCP.Length == 0)
                cmd.CPData = null;//无数据
            else
            {
                // 初始化CP数据存储的实例对象
                cmd.CPData = new CmdCPData();
                cmd.CPData.CPDataItems = new List<CmdCPDataItem>();
                //添加一个分号，以便后面匹配时方便查找
                if (!sCmdCP.EndsWith(";"))
                    sCmdCP += ";";
                string sCpData = sCmdCP;
                //提取QN值，如果有的话，也可能没有；
                sValue = DecodeCmd_CP_GetSpecialItemValue(ref sCpData, @"QN=\S*?;");
                if (sValue.Length > 0)
                {
                    cmd.CPData.QN = sValue.Substring(3, sValue.Length - 4);
                }
                //提取QnRtn
                sValue = DecodeCmd_CP_GetSpecialItemValue(ref sCpData, @"QnRtn=\S*?;");
                if (sValue.Length > 0)
                {
                    sValue = sValue.Substring(6, sValue.Length - 7);
                    if (!int.TryParse(sValue, out iValue))
                    {
                        sErr = $"收到的命令解析QnRtn时,对应值为{sValue}不是有效整数，无法解析。[{sData}]";
                        return null;
                    }
                    cmd.CPData.QnRtn = (QnRtnValues)iValue;
                }
                //提取ExeRtn
                sValue = DecodeCmd_CP_GetSpecialItemValue(ref sCpData, @"ExeRtn=\S*?;");
                if (sValue.Length > 0)
                {
                    sValue = sValue.Substring(7, sValue.Length - 8);
                    if (!int.TryParse(sValue, out iValue))
                    {
                        sErr = $"收到的命令解析ExeRtn时,对应值为{sValue}不是有效整数，无法解析。[{sData}]";
                        return null;
                    }
                    cmd.CPData.ExeRtn = (ExeRtnValues)iValue;
                }
                //开始读取自定义字段了，这个区分SNO和ENO的了
                List<string> listSN = DecodeCmd_CP_GetSpliter(sCpData, @"SNO=\S*?;");//返回多个SNO字符窜；
                List<string> listENO;
                string sSNo;
                string sENo;
                for (int i = 0; i < listSN.Count; i++)
                {
                    string sSNAllData = listSN[i];
                    sSNo = DecodeCmd_CP_GetSpecialItemValue(ref sSNAllData, @"SNO=\S*?;");//获取SN编码，也有可能没有的，这个无所谓
                    if (sSNo.Length >= 5) sSNo = sSNo.Substring(4, sSNo.Length - 5);
                    listENO = DecodeCmd_CP_GetSpliter(sSNAllData, @"ENO=\S*?;");//返回多个ENO字符窜；
                    for (int j = 0; j < listENO.Count; j++)
                    {
                        CmdCPDataItem cmdCPDataItem = new CmdCPDataItem();
                        cmdCPDataItem.SNO = sSNo;
                        //此时读取各设备数据
                        string sENAllData = listENO[j];
                        sENo = DecodeCmd_CP_GetSpecialItemValue(ref sENAllData, @"ENO=\S*?;");//获取EN编码，也有可能没有的，这个无所谓
                        if (sENo.Length >= 5)
                            sENo = sENo.Substring(4, sENo.Length - 5);
                        cmdCPDataItem.ENo = sENo;
                        //后面的数据属于自定义字段了，目前不清楚会是什么，也不需要清楚；
                        string[] arrCPItems = sENAllData.Split(';');
                        foreach (string sCPItems in arrCPItems)
                        {
                            if (sCPItems.Length == 0) continue;
                            if (!DecodeCmd_SplitItem(sCPItems, out sName, out sValue, out sErr))
                            {
                                sErr = $"收到的命令解析[{sCPItems}]时出错：{sErr}，无法解析。[{sData}]";
                                return null;
                            }
                            cmdCPDataItem.AddItem(sName, sValue);
                        }
                        cmd.CPData.CPDataItems.Add(cmdCPDataItem);
                    }
                }
            }
            #endregion
            sErr = string.Empty;
            return cmd;
        }
        private static bool DecodeCmd_SplitItem(string sSource, out string sItemName, out string sItemValue, out string sErr)
        {
            /************
             * 提取内容XXX=aaaa；XXX是项目名称，aaaa是项目值，注意这里不用split函数，因为aaaa只能可能包含=；
             * ***********/
            int iIndex = sSource.IndexOf("=");
            if (iIndex < 0)
            {
                sErr = $"传入的文本未包含分割符\"=\"";
                sItemName = string.Empty;
                sItemValue = string.Empty;
                return false;
            }
            sItemName = sSource.Substring(0, iIndex);
            if (iIndex >= sSource.Length - 1) sItemValue = string.Empty;//此时=后面没有数据了；
            else
            {
                iIndex++;
                sItemValue = sSource.Substring(iIndex, sSource.Length - iIndex);
            }
            sErr = string.Empty;
            return true;
        }
        /// <summary>
        /// 找到指定项目的内容，完成后将该项目从文本中删除
        /// </summary>
        /// <param name="sCPData"></param>
        /// <param name="sPattern"></param>
        private static string DecodeCmd_CP_GetSpecialItemValue(ref string sCPData, string sPattern)
        {
            string sValue = AppHelper.AppFuns.FindTextByRegex(sCPData, sPattern);
            if (sValue.Length > 0)
            {
                sCPData = sCPData.Replace(sValue, "");
            }
            return sValue;
        }
        private static List<string> DecodeCmd_CP_GetSpliter(string sCpData, string sPatternItem)
        {
            List<string> listRtn = new List<string>();
            int iSNoStart, iSNoEnd;
            iSNoStart = 0;//第一次查找的起始位置
            while (true)
            {
                iSNoStart = AppHelper.AppFuns.FindTextFirstIndexByRegex(sCpData, sPatternItem, iSNoStart);
                if (iSNoStart < 0)
                {
                    //此时不包含SNO，这也是可能的，以为如果只有1个站点的话，发过来的数据可能会省去这个标识，或者一些统一命令，也不包含这个
                    iSNoStart = 0;
                    iSNoEnd = sCpData.Length;//表示整段字符都属于该SN，只是这个SN没有编码
                }
                else
                {
                    //此时后面有可能还有SNO，也有可能没有SNO
                    iSNoEnd = AppHelper.AppFuns.FindTextFirstIndexByRegex(sCpData, sPatternItem, iSNoStart + 1);//这里要+1，否则还是会搜索到当前的这个位置；
                    if (iSNoEnd < 0)
                        iSNoEnd = sCpData.Length;//此时表示后面没有SNO了
                }
                //读取数据
                string strItems = sCpData.Substring(iSNoStart, iSNoEnd - iSNoStart);
                if (strItems.Length > 0)
                    listRtn.Add(strItems);
                if (iSNoEnd >= sCpData.Length) break;
                iSNoStart = iSNoEnd;//下一个起始位置，为当前的结束位置；
            }
            return listRtn;
        }
        #endregion
    }
}
