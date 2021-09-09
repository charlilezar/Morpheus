using Aming.Core;
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

namespace HengDa.LiZongMing.REAMS.Wpf.UC
{
    /// <summary>
    /// UcHDZJCView.xaml 的交互逻辑
    /// </summary>
    public partial class UcHDZJCView : UserControl
    {
        List<Control> _Controls = null;
        public UcHDZJCView()
        {
            InitializeComponent();
            _Controls = new List<Control>();
            _Controls.Add(this.labMacNo);
            _Controls.Add(this.tbMacNo);
            _Controls.Add(this.labSelCommMode);
            _Controls.Add(this.comComMode);
            _Controls.Add(this.labSelCmd);
            _Controls.Add(this.comCmds);
            _Controls.Add(this.labRainDetailSearchDate);
            _Controls.Add(this.tbRainDetailSearchDate);
            _Controls.Add(this.labAlarmDetailSearchDate);
            _Controls.Add(this.tbAlarmDetailSearchDate);
            _Controls.Add(this.labDryDepDetailStartIndex);
            _Controls.Add(this.tbDryDepDetailStartIndex);
            _Controls.Add(this.tbRainDetailStartIndex);
            _Controls.Add(this.labDryDepDetailSearchCnt);
            _Controls.Add(this.tbDryDepDetailSearchCnt);
            _Controls.Add(this.btExcute);
            _Controls.Add(this.chkAutoExcute);
            ShowGrid(ShowGridTypes.None);
            SetParamtersVisiblity_None();
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NameValue<HDZJCCmdCode> item = this.comCmds.SelectedItem as NameValue<HDZJCCmdCode>;
            if (item == null) return;
            switch (item.Value)
            {
                case HDZJCCmdCode.实时数据:
                    SetParamtersVisiblity_None();
                    ShowGrid(ShowGridTypes.None);
                    break;
                case HDZJCCmdCode.干沉降记录查询:
                    SetParamtersVisiblity_None();
                    ShowGrid(ShowGridTypes.DryDetail);
                    break;
                case HDZJCCmdCode.设置干沉降条号索引条号:
                    SetParamtersVisiblity_DryDetailIndex();
                    ShowGrid(ShowGridTypes.None);
                    break;
                case HDZJCCmdCode.查询指定干沉降索引号的干沉降记录:
                    SetParamtersVisiblity_DryDetailIndex();
                    ShowGrid(ShowGridTypes.DryDetail);
                    break;
                case HDZJCCmdCode.故障记录查询:
                    SetParamtersVisiblity_None();
                    ShowGrid(ShowGridTypes.AlarmDetail);
                    break;
                case HDZJCCmdCode.设置故障查询日期索引:
                    SetParamtersVisiblity_AlarmDetailDate();
                    ShowGrid(ShowGridTypes.None);
                    break;
                case HDZJCCmdCode.查询指定时间的故障记录明细:
                    SetParamtersVisiblity_AlarmDetailDate();
                    ShowGrid(ShowGridTypes.AlarmDetail);
                    break;
                case HDZJCCmdCode.设置降雨明条号细索引:
                    SetParamtersVisiblity_RainDetailIndex();
                    ShowGrid(ShowGridTypes.None);
                    break;
                case HDZJCCmdCode.设置降雨明细日期索引:
                    SetParamtersVisiblity_RainDetailDate();
                    ShowGrid(ShowGridTypes.None);
                    break;
                case HDZJCCmdCode.降雨记录查询:
                    SetParamtersVisiblity_None();
                    ShowGrid(ShowGridTypes.RainDetail);
                    break;
                case HDZJCCmdCode.查询指定时间的降雨记录明细://该查询目前不指定索引
                    SetParamtersVisiblity_RainDetailDate();
                    ShowGrid(ShowGridTypes.RainDetail);
                    break;
                case HDZJCCmdCode.自定义地址查询://该查询目前不指定索引
                    SetParamtersVisiblity_CutomAdr();
                    ShowGrid(ShowGridTypes.None);
                    break;
                default:
                    SetParamtersVisiblity_None();
                    break;
            }
        }
        private void SetParamtersVisiblity_RainDetailDate()
        {
            this.labRainDetailSearchDate.Visibility = Visibility.Visible;
            this.tbRainDetailSearchDate.Visibility = Visibility.Visible;
            this.labAlarmDetailSearchDate.Visibility = Visibility.Hidden;
            this.tbAlarmDetailSearchDate.Visibility = Visibility.Hidden;
            this.labDryDepDetailStartIndex.Visibility = Visibility.Hidden;
            this.tbDryDepDetailStartIndex.Visibility = Visibility.Hidden;
            this.labRainDetailStartIndex.Visibility = Visibility.Hidden;
            this.tbRainDetailStartIndex.Visibility = Visibility.Hidden;
            this.labDryDepDetailSearchCnt.Visibility = Visibility.Hidden;
            this.tbDryDepDetailSearchCnt.Visibility = Visibility.Hidden;
            ControlStyle();
        }
        private void SetParamtersVisiblity_None()
        {
            this.labRainDetailSearchDate.Visibility = Visibility.Hidden;
            this.tbRainDetailSearchDate.Visibility = Visibility.Hidden;
            this.labAlarmDetailSearchDate.Visibility = Visibility.Hidden;
            this.tbAlarmDetailSearchDate.Visibility = Visibility.Hidden;
            this.labDryDepDetailStartIndex.Visibility = Visibility.Hidden;
            this.tbDryDepDetailStartIndex.Visibility = Visibility.Hidden;
            this.labRainDetailStartIndex.Visibility = Visibility.Hidden;
            this.tbRainDetailStartIndex.Visibility = Visibility.Hidden;
            this.labDryDepDetailSearchCnt.Visibility = Visibility.Hidden;
            this.tbDryDepDetailSearchCnt.Visibility = Visibility.Hidden;
            ControlStyle();
        }
        private void SetParamtersVisiblity_AlarmDetailDate()
        {
            this.labRainDetailSearchDate.Visibility = Visibility.Hidden;
            this.tbRainDetailSearchDate.Visibility = Visibility.Hidden;
            this.labAlarmDetailSearchDate.Visibility = Visibility.Visible;
            this.tbAlarmDetailSearchDate.Visibility = Visibility.Visible;
            this.labDryDepDetailStartIndex.Visibility = Visibility.Hidden;
            this.tbDryDepDetailStartIndex.Visibility = Visibility.Hidden;
            this.labRainDetailStartIndex.Visibility = Visibility.Hidden;
            this.tbRainDetailStartIndex.Visibility = Visibility.Hidden;
            this.labDryDepDetailSearchCnt.Visibility = Visibility.Hidden;
            this.tbDryDepDetailSearchCnt.Visibility = Visibility.Hidden;
            ControlStyle();
        }
        private void SetParamtersVisiblity_DryDetailIndex()
        {
            this.labRainDetailSearchDate.Visibility = Visibility.Hidden;
            this.tbRainDetailSearchDate.Visibility = Visibility.Hidden;
            this.labAlarmDetailSearchDate.Visibility = Visibility.Hidden;
            this.tbAlarmDetailSearchDate.Visibility = Visibility.Hidden;
            this.labDryDepDetailStartIndex.Visibility = Visibility.Visible;
            labDryDepDetailStartIndex.Content = "干沉降索引号";
            this.tbDryDepDetailStartIndex.Visibility = Visibility.Visible;
            this.labRainDetailStartIndex.Visibility = Visibility.Hidden;
            this.tbRainDetailStartIndex.Visibility = Visibility.Hidden;
            this.labDryDepDetailSearchCnt.Visibility = Visibility.Visible;
            this.tbDryDepDetailSearchCnt.Visibility = Visibility.Visible;
            ControlStyle();
        }
        private void SetParamtersVisiblity_CutomAdr()
        {
            this.labRainDetailSearchDate.Visibility = Visibility.Hidden;
            this.tbRainDetailSearchDate.Visibility = Visibility.Hidden;
            this.labAlarmDetailSearchDate.Visibility = Visibility.Hidden;
            this.tbAlarmDetailSearchDate.Visibility = Visibility.Hidden;
            this.labDryDepDetailStartIndex.Visibility = Visibility.Visible;
            labDryDepDetailStartIndex.Content = "起始地址";
            this.tbDryDepDetailStartIndex.Visibility = Visibility.Visible;
            this.labRainDetailStartIndex.Visibility = Visibility.Hidden;
            this.tbRainDetailStartIndex.Visibility = Visibility.Hidden;
            this.labDryDepDetailSearchCnt.Visibility = Visibility.Hidden;
            this.tbDryDepDetailSearchCnt.Visibility = Visibility.Visible;
            ControlStyle();
        }
        private void SetParamtersVisiblity_RainDetailIndex()
        {
            this.labRainDetailSearchDate.Visibility = Visibility.Hidden;
            this.tbRainDetailSearchDate.Visibility = Visibility.Hidden;
            this.labAlarmDetailSearchDate.Visibility = Visibility.Hidden;
            this.tbAlarmDetailSearchDate.Visibility = Visibility.Hidden;
            this.labDryDepDetailStartIndex.Visibility = Visibility.Hidden;
            this.tbDryDepDetailStartIndex.Visibility = Visibility.Hidden;
            this.labRainDetailStartIndex.Visibility = Visibility.Visible;
            this.tbRainDetailStartIndex.Visibility = Visibility.Visible;
            this.labDryDepDetailSearchCnt.Visibility = Visibility.Hidden;
            this.tbDryDepDetailSearchCnt.Visibility = Visibility.Hidden;

            ControlStyle();
        }
        private void ShowGrid(ShowGridTypes showType)
        {
            if (showType == ShowGridTypes.RainDetail)
            {
                this.dgRainDetail.Visibility = Visibility.Visible;
                this.dgAlarmDetail.Visibility = Visibility.Hidden;
                this.dgDryDetail.Visibility = Visibility.Hidden;
                this.dgResult.Visibility = Visibility.Hidden;
            }
            else if (showType == ShowGridTypes.AlarmDetail)
            {
                this.dgRainDetail.Visibility = Visibility.Hidden;
                this.dgAlarmDetail.Visibility = Visibility.Visible;
                this.dgDryDetail.Visibility = Visibility.Hidden;
                this.dgResult.Visibility = Visibility.Hidden;
            }
            else if (showType == ShowGridTypes.DryDetail)
            {
                this.dgRainDetail.Visibility = Visibility.Hidden;
                this.dgAlarmDetail.Visibility = Visibility.Hidden;
                this.dgDryDetail.Visibility = Visibility.Visible;
                this.dgResult.Visibility = Visibility.Hidden;
            }
            else
            {
                this.dgRainDetail.Visibility = Visibility.Hidden;
                this.dgAlarmDetail.Visibility = Visibility.Hidden;
                this.dgDryDetail.Visibility = Visibility.Hidden;
                this.dgResult.Visibility = Visibility.Visible;
            }
        }
        private void ControlStyle()
        {
            double dbMargin = 3;
            double dbLef = dbMargin;
            double dbTop = 3;
            //iTop = this.comCmds.Margin.Top;
            //iRight = this.comCmds.Margin.Right;
            //iBottom = this.comCmds.Margin.Bottom;
            foreach (Control con in this._Controls)
            {
                if (con.Visibility == Visibility.Visible)
                {
                    if (string.Compare(con.Name, "chkAutoExcute", true) == 0)
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
        #region 相关类及枚举
        private enum ShowGridTypes
        {
            None = 0,
            RainDetail = 1,
            AlarmDetail = 2,
            DryDetail = 3
        }
        #endregion

        private void mytest_Click(object sender, RoutedEventArgs e)
        {
            //不知道为什么model中新增了行后，界面就是不显示新增的行，用下面的刷新一下就好了
            System.Collections.IEnumerable obj = this.dgRainDetail.ItemsSource;
            this.dgRainDetail.ItemsSource = null;
            this.dgRainDetail.ItemsSource = obj;
        }
    }
}
