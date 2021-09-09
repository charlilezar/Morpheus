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
    /// UcHDRNSView.xaml 的交互逻辑
    /// </summary>
    public partial class UcHDRNSView : UserControl
    {
        List<Control> _Controls = null;
        public UcHDRNSView()
        {
            InitializeComponent();
            _Controls = new List<Control>();
            _Controls.Add(this.labMacNo);
            _Controls.Add(this.tbMacNo);
            _Controls.Add(this.labSelCommMode);
            _Controls.Add(this.comComMode);
            _Controls.Add(this.labSelCmd);
            _Controls.Add(this.comCmds);
            _Controls.Add(this.labRainingTmpMaxSet);
            _Controls.Add(this.tbRainingTmpMaxSet);
            _Controls.Add(this.labUnRainTmpMaxSet);
            _Controls.Add(this.tbUnRainTmpMaxSet);
            _Controls.Add(this.btExcute);
            _Controls.Add(this.chkAutoExcute);
            SetParamtersVisiblity_None();
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NameValue<HDRNSCmdCode> item = this.comCmds.SelectedItem as NameValue<HDRNSCmdCode>;
            if (item == null) return;
            switch (item.Value)
            {
                case HDRNSCmdCode.实时数据:
                    SetParamtersVisiblity_None();
                    break;
                case HDRNSCmdCode.设置下雨加热温度阀值:
                    this.SetParamtersVisiblity_RainingTmpMaxSet();
                    break;
                case HDRNSCmdCode.设置不下雨加热温度阀值:
                    this.SetParamtersVisiblity_UnRainTmpMaxSet();
                    break;
                case HDRNSCmdCode.设置下雨及不下雨加热温度阀值:
                    SetParamtersVisiblity_BothTmpMaxSet();
                    break;
                default:
                    SetParamtersVisiblity_None();
                    break;
            }
        }
        private void SetParamtersVisiblity_RainingTmpMaxSet()
        {
            this.labRainingTmpMaxSet.Visibility = Visibility.Visible;
            this.tbRainingTmpMaxSet.Visibility = Visibility.Visible;
            this.labUnRainTmpMaxSet.Visibility = Visibility.Hidden;
            this.tbUnRainTmpMaxSet.Visibility = Visibility.Hidden;
           
            ControlStyle();
        }
        private void SetParamtersVisiblity_BothTmpMaxSet()
        {
            this.labRainingTmpMaxSet.Visibility = Visibility.Visible;
            this.tbRainingTmpMaxSet.Visibility = Visibility.Visible;
            this.labUnRainTmpMaxSet.Visibility = Visibility.Visible;
            this.tbUnRainTmpMaxSet.Visibility = Visibility.Visible;

            ControlStyle();
        }
        private void SetParamtersVisiblity_None()
        {
            this.labRainingTmpMaxSet.Visibility = Visibility.Hidden;
            this.tbRainingTmpMaxSet.Visibility = Visibility.Hidden;
            this.labUnRainTmpMaxSet.Visibility = Visibility.Hidden;
            this.tbUnRainTmpMaxSet.Visibility = Visibility.Hidden;
            ControlStyle();
        }
        private void SetParamtersVisiblity_UnRainTmpMaxSet()
        {
            this.labRainingTmpMaxSet.Visibility = Visibility.Hidden;
            this.tbRainingTmpMaxSet.Visibility = Visibility.Hidden;
            this.labUnRainTmpMaxSet.Visibility = Visibility.Visible;
            this.tbUnRainTmpMaxSet.Visibility = Visibility.Visible;
            ControlStyle();
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
    }
}
