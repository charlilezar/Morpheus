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
    public partial class UcHDZCQView : UserControl
    {
        public UcHDZCQView()
        {
            InitializeComponent();
            _Controls = new List<Control>();
            _Controls.Add(this.labSelCommMode);
            _Controls.Add(this.comComMode);
            _Controls.Add(this.labSelCmd);
            _Controls.Add(this.comCmds);
            _Controls.Add(this.labCmdParameter1);
            _Controls.Add(this.comCaiYangData);
            _Controls.Add(this.tbCmdParameter1);
            _Controls.Add(this.labCmdParameter2);
            _Controls.Add(this.tbCmdParameter2);
            _Controls.Add(this.btExcute);
            _Controls.Add(this.chkAutoExcute);
            this.SetControlStyle_NoneParamter();
        }
        List<Control> _Controls = null;
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NameValue<HDZCQCmdCode> item = this.comCmds.SelectedItem as NameValue<HDZCQCmdCode>;
            if (item == null) return;
            switch (item.Value)
            {
                case HDZCQCmdCode.开始采样:
                    SetControlStyle_OnlyCombobox("采样参数");
                    break;
                case HDZCQCmdCode.读取文件:
                    SetControlStyle_OnlyOneParamter("文件序号");
                    break;
                case HDZCQCmdCode.采样瞬时流量设定:
                    SetControlStyle_OnlyOneParamter("流量设定", 120);
                    break;
                case HDZCQCmdCode.采样时间设定:
                    SetControlStyle_OnlyTwoParamter("小时设定","分钟设定", 80, 80);
                    break;
                case HDZCQCmdCode.定量采样量设定:
                    SetControlStyle_OnlyOneParamter("采样量设定",80);
                    break;
                case HDZCQCmdCode.时间设置:
                    SetControlStyle_OnlyOneParamter("时间设置", 200);
                    this.tbCmdParameter1.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case HDZCQCmdCode.滤膜编号设定:
                    SetControlStyle_OnlyOneParamter("滤膜编号", 250);
                    break;
                case HDZCQCmdCode.采样次数:
                    SetControlStyle_OnlyOneParamter("采样次数", 80);
                    break;
                case HDZCQCmdCode.定时采样间隔:
                    SetControlStyle_OnlyTwoParamter("小时设定", "分钟设定", 80, 80);
                    break;
                case HDZCQCmdCode.定时启动时间:
                    SetControlStyle_OnlyOneParamter("启动时间值", 200);
                    this.tbCmdParameter1.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    break;
                case HDZCQCmdCode.定时启动使能:
                    SetControlStyle_OnlyOneParamter("输入(0或1)", 60);
                    this.tbCmdParameter1.Text = "1";
                    break;
                default:
                    SetControlStyle_NoneParamter();
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
        private void SetControlStyle_OnlyCombobox(string sTitle)
        {
            //显示部分
            this.labCmdParameter1.Content = sTitle;
            this.labCmdParameter1.Visibility = Visibility.Visible;
            this.comCaiYangData.Visibility = Visibility.Visible;
            //隐藏部分
            this.tbCmdParameter1.Visibility = Visibility.Hidden;
            this.labCmdParameter2.Visibility = Visibility.Hidden;
            this.tbCmdParameter2.Visibility = Visibility.Hidden;
            this.ControlStyle();
        }
        private void SetControlStyle_NoneParamter()
        {
            //隐藏所有
            this.labCmdParameter1.Visibility = Visibility.Hidden;
            this.comCaiYangData.Visibility = Visibility.Hidden;
            this.tbCmdParameter1.Visibility = Visibility.Hidden;
            this.labCmdParameter2.Visibility = Visibility.Hidden; 
            this.tbCmdParameter2.Visibility = Visibility.Hidden;
            this.ControlStyle();


        }
        
        private void SetControlStyle_OnlyOneParamter(string sTitle, int iWidth= 120)
        {

            this.labCmdParameter1.Content = sTitle;
            //显示部分
            this.labCmdParameter1.Visibility = Visibility.Visible;
            this.tbCmdParameter1.Visibility = Visibility.Visible;
            //隐藏部分
            this.comCaiYangData.Visibility = Visibility.Hidden;
            this.labCmdParameter2.Visibility = Visibility.Hidden;
            this.tbCmdParameter2.Visibility = Visibility.Hidden;
            //设置宽度
            this.tbCmdParameter1.Width = iWidth;
            this.ControlStyle();
        }
        private void SetControlStyle_OnlyTwoParamter(string sTitle,string sTitle1, int iWidth = 120, int iWidth1 = 120)
        {
            this.labCmdParameter1.Content = sTitle;
            this.labCmdParameter2.Content = sTitle1;
            //显示部分
            this.labCmdParameter1.Visibility = Visibility.Visible;
            this.tbCmdParameter1.Visibility = Visibility.Visible;
            this.labCmdParameter2.Visibility = Visibility.Visible;
            this.tbCmdParameter2.Visibility = Visibility.Visible;
            //隐藏部分
            this.comCaiYangData.Visibility = Visibility.Hidden;
            //设置宽度
            this.tbCmdParameter1.Width = iWidth;
            this.tbCmdParameter1.Width = iWidth1;
            this.ControlStyle();
        }
    }
}
