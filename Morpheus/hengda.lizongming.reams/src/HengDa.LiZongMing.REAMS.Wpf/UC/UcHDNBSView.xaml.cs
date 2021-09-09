using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Aming.Core;

namespace HengDa.LiZongMing.REAMS.Wpf.UC
{
    /// <summary>
    /// UcHDZCQ.xaml 的交互逻辑
    /// </summary>
    public partial class UcHDNBSView : UserControl
    {
        public UcHDNBSView()
        {
            InitializeComponent();
            _Controls = new List<Control>();
            _Controls.Add(labMacNo);
            _Controls.Add(tbMacNo);
            _Controls.Add(this.labSelCommMode);
            _Controls.Add(this.comComMode);
            _Controls.Add(this.labSelCmd);
            _Controls.Add(this.comCmds);
            _Controls.Add(this.labCmdParameter1);
            _Controls.Add(this.comInstController);
            _Controls.Add(comWorkMode);
            _Controls.Add(this.tbSamplingVSet);
            _Controls.Add(this.tbSampllingTimeLongSetHours);
            _Controls.Add(this.labCmdParameter2);
            _Controls.Add(this.tbSampllingTimeLongSetMinutes);
            _Controls.Add(this.tbSampllingFlowRateSet);
            _Controls.Add(this.tbLuMoID);
            _Controls.Add(this.tbInstrumentTimeSet);
            _Controls.Add(this.tbFileNo);
            _Controls.Add(this.btExcute);
            _Controls.Add(this.chkAutoExcute);
            this.SetParamtersVisible(string.Empty, null);
        }
        List<Control> _Controls = null;
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NameValue<HDNBSCmdCode> item = this.comCmds.SelectedItem as NameValue<HDNBSCmdCode>;
            if (item == null) return;
            switch (item.Value)
            {
                case HDNBSCmdCode.采样启停控制://1
                    this.SetParamtersVisible("控制方式", this.comInstController);
                    break;
                case HDNBSCmdCode.时间设置://2
                    this.SetParamtersVisible("设置时间", this.tbInstrumentTimeSet);
                    this.tbInstrumentTimeSet.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case HDNBSCmdCode.NBS运行模式设置://3
                    this.SetParamtersVisible("运行模式", this.comWorkMode);
                    break;
                case HDNBSCmdCode.滤膜编号设定://4
                    this.SetParamtersVisible("滤膜ID", this.tbLuMoID);
                    break;
                case HDNBSCmdCode.采样流量设定://5
                    this.SetParamtersVisible("流量设定", this.tbSampllingFlowRateSet);
                    break;
                case HDNBSCmdCode.采样时长设定://6
                    this.SetParamtersVisible("采样时长(h)", "采样时长(m)", this.tbSampllingTimeLongSetHours,this.tbSampllingTimeLongSetMinutes);
                    break;
                case HDNBSCmdCode.采样体积设定://7
                    this.SetParamtersVisible("体积设定", this.tbSamplingVSet);
                    break;
                case HDNBSCmdCode.读取文件://16
                    this.SetParamtersVisible("文件号", this.tbFileNo);
                    break;
                default:
                    this.SetParamtersVisible(string.Empty, null);
                    break;
            }
        }
        private void ControlStyle()
        {
            double dbMargin = 5;
            double dbLef = dbMargin;
            double dbTop = 3;
            //iTop = this.comCmds.Margin.Top;
            //iRight = this.comCmds.Margin.Right;
            //iBottom = this.comCmds.Margin.Bottom;
            foreach (Control con in this._Controls)
            {
                if (con.Visibility == Visibility.Visible)
                {
                    if(string.Compare(con.Name, "chkAutoExcute",true)==0)
                    {
                        dbTop = 7;
                        dbLef += 13;
                    }
                    con.SetValue(Canvas.TopProperty, dbTop);
                    con.SetValue(Canvas.LeftProperty, dbLef);
                    dbLef += con.Width + dbMargin;
                }
            }
        }
        private void SetParamtersVisible(string sTitle1, Control con1)
        {
            SetParamtersVisible(sTitle1, string.Empty, con1, null);
        }
        private void SetParamtersVisible(string sTitle1,string sTitle2,Control con1,Control con2)
        {
            this.labCmdParameter1.Visibility = sTitle1.Length > 0 ? Visibility.Visible : Visibility.Hidden;
            this.labCmdParameter2.Visibility = sTitle1.Length > 0 ? Visibility.Visible : Visibility.Hidden;
            this.labCmdParameter1.Content = sTitle1;
            this.labCmdParameter2.Content = sTitle2;
            for (int i = 6; i < this._Controls.Count-2; i++)
            {
                Control con = this._Controls[i];
                if (con1 != null && con == con1)
                    con.Visibility = Visibility.Visible;
                else if (con2 != null && con == con2)
                    con.Visibility = Visibility.Visible;
                else con.Visibility = Visibility.Hidden;
            }
            this.ControlStyle();
        }
    }
}
