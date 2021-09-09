using System;
using System.Windows.Media;

using Stylet;

namespace HengDa.LiZongMing.REAMS.ViewModel
{
    public class ColoredLableViewModel : PropertyChangedBase
    {
        public static ColoredLableViewModel FromStatus(InstrumentStatus sts)
        {
            var vm = new ColoredLableViewModel();
            vm.Connected = sts== InstrumentStatus.Connected;
            switch (sts)
            {
                case InstrumentStatus.Connected:
                    vm.Text = "已连接";
                    vm.Foreground = Brushes.LimeGreen;
                    break;
                case InstrumentStatus.Connecting:
                    vm.Text = "正在连接";
                    vm.Foreground = Brushes.Gold;
                    break;
                case InstrumentStatus.NotConnected:
                    vm.Text = "未连接";
                    vm.Foreground = Brushes.Red;
                    break;
                case InstrumentStatus.Unknow:
                    vm.Text = "未知";
                    vm.Foreground = Brushes.Gray;
                    break;
            }
            return vm;
        }

        private string _text;

        public string Text
        {
            get => _text;
            set
            {
                if (_text == value) return;
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        private Brush _foreground;

        public Brush Foreground
        {
            get => _foreground;
            set
            {
                if (value.Equals(_foreground)) return;
                _foreground = value;
                OnPropertyChanged(nameof(Foreground));
            }
        }

        public bool Connected { get; private set; }
    }
}