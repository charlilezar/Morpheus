using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using HengDa.LiZongMing.REAMS.ViewModel;
using HengDa.LiZongMing.REAMS.Wpf.UC;
using Stylet;

namespace HengDa.LiZongMing.REAMS
{
    class RootViewModel : Screen
    {
        private readonly IServiceProvider ServiceProvider;
        private IWindowManager windowManager;



        public RootViewModel(IServiceProvider serviceProvider, IWindowManager windowManager)
        {
            //this.DisplayName = "";
            this.ServiceProvider = serviceProvider;
            this.windowManager = windowManager;
            this.vmHDZCQ = ServiceProvider.GetService<UcHDZCQViewModel>();
            this.vmHDZJC = ServiceProvider.GetService<UcHDZJCViewModel>();
            //this.vmHDRNS = ServiceProvider.GetService<UcHDRNSViewModel>();
            //this.vmHDNBS = ServiceProvider.GetService<UcHDNBSViewModel>();

            //重点，
            Aming.Tools.IocHelper.ServiceProvider = serviceProvider;
        }

        public ColoredLableViewModel ZCQStatus { get; private set; } = ColoredLableViewModel.FromStatus(InstrumentStatus.Connected);
        #region 各用户控件的数据ViewModel
        /// <summary>
        /// 气碘采样ViewModel
        /// </summary>
        public UcHDZCQViewModel vmHDZCQ { get; set; }
        ///// <summary>
        ///// 干湿沉降的ViewModel
        ///// </summary>
        public UcHDZJCViewModel vmHDZJC { get; set; } 
        ///// <summary>
        ///// 感雨器的ViewModel
        ///// </summary>
        //public UcHDRNSViewModel vmHDRNS { get; set; }
        ///// <summary>
        ///// 超大容量气溶胶ViewModel
        ///// </summary>
        //public UcHDNBSViewModel vmHDNBS { get; set; }
        #endregion

        /// <summary>
        /// 退出
        /// </summary>
        public ICommand CmdExitApp { get; set; }

        private void ExitApp()
        {

            //退出程序
            Application.Current.Shutdown(); // .MainWindow.Close();

        }
    }
}