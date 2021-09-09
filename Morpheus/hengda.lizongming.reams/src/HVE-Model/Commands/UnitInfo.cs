using System;

namespace Model.Commands
{
	// Token: 0x02000018 RID: 24
	public class UnitInfo
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000068 RID: 104 RVA: 0x00002505 File Offset: 0x00000705
		// (set) Token: 0x06000069 RID: 105 RVA: 0x0000250D File Offset: 0x0000070D
		public double TemperatureSensitivity { get; set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600006B RID: 107 RVA: 0x0000251F File Offset: 0x0000071F
		// (set) Token: 0x0600006A RID: 106 RVA: 0x00002516 File Offset: 0x00000716
		public string FirmwareRevision { get; set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600006D RID: 109 RVA: 0x00002530 File Offset: 0x00000730
		// (set) Token: 0x0600006C RID: 108 RVA: 0x00002527 File Offset: 0x00000727
		public string OSVersion { get; set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600006F RID: 111 RVA: 0x00002541 File Offset: 0x00000741
		// (set) Token: 0x0600006E RID: 110 RVA: 0x00002538 File Offset: 0x00000738
		public string MacAddress { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000071 RID: 113 RVA: 0x00002552 File Offset: 0x00000752
		// (set) Token: 0x06000070 RID: 112 RVA: 0x00002549 File Offset: 0x00000749
		public string Unused { get; set; }

		// Token: 0x06000072 RID: 114 RVA: 0x0000255C File Offset: 0x0000075C
		public UnitInfo()
		{
			UnitInfo.Instance = this;
			this._unitName = "Unknown";
			this._unitVersion = "Unknown";
			this._unitSerialNumber = "Unknown";
			this._daqSerialNumber = "Unknown";
			this._electrometerSerialNumber = "Unknown";
			this._hpicSerialNumber = "Unknown";
			this._daqVersion = "Unknown";
			this.TemperatureSensitivity = 0.0;
			this.Unused = " ";
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000074 RID: 116 RVA: 0x000025FF File Offset: 0x000007FF
		// (set) Token: 0x06000073 RID: 115 RVA: 0x000025DC File Offset: 0x000007DC
		public string UnitName
		{
			get
			{
				return this._unitName;
			}
			set
			{
				if (value.Length > 20)
				{
					this._unitName = value.Substring(0, 20);
					return;
				}
				this._unitName = value;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000076 RID: 118 RVA: 0x0000262A File Offset: 0x0000082A
		// (set) Token: 0x06000075 RID: 117 RVA: 0x00002607 File Offset: 0x00000807
		public string UnitSerialNumber
		{
			get
			{
				return this._unitSerialNumber;
			}
			set
			{
				if (value.Length > 20)
				{
					this._unitSerialNumber = value.Substring(0, 20);
					return;
				}
				this._unitSerialNumber = value;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000078 RID: 120 RVA: 0x00002655 File Offset: 0x00000855
		// (set) Token: 0x06000077 RID: 119 RVA: 0x00002632 File Offset: 0x00000832
		public string DaqSerialNumber
		{
			get
			{
				return this._daqSerialNumber;
			}
			set
			{
				if (value.Length > 20)
				{
					this._daqSerialNumber = value.Substring(0, 20);
					return;
				}
				this._daqSerialNumber = value;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600007A RID: 122 RVA: 0x00002680 File Offset: 0x00000880
		// (set) Token: 0x06000079 RID: 121 RVA: 0x0000265D File Offset: 0x0000085D
		public string HpicSerialNumber
		{
			get
			{
				return this._hpicSerialNumber;
			}
			set
			{
				if (value.Length > 20)
				{
					this._hpicSerialNumber = value.Substring(0, 20);
					return;
				}
				this._hpicSerialNumber = value;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600007C RID: 124 RVA: 0x000026AB File Offset: 0x000008AB
		// (set) Token: 0x0600007B RID: 123 RVA: 0x00002688 File Offset: 0x00000888
		public string ElectrometerSerialNumber
		{
			get
			{
				return this._electrometerSerialNumber;
			}
			set
			{
				if (value.Length > 20)
				{
					this._electrometerSerialNumber = value.Substring(0, 20);
					return;
				}
				this._electrometerSerialNumber = value;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600007E RID: 126 RVA: 0x000026D6 File Offset: 0x000008D6
		// (set) Token: 0x0600007D RID: 125 RVA: 0x000026B3 File Offset: 0x000008B3
		public string UnitVersion
		{
			get
			{
				return this._unitVersion;
			}
			set
			{
				if (value.Length > 20)
				{
					this._unitVersion = value.Substring(0, 20);
					return;
				}
				this._unitVersion = value;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000080 RID: 128 RVA: 0x00002701 File Offset: 0x00000901
		// (set) Token: 0x0600007F RID: 127 RVA: 0x000026DE File Offset: 0x000008DE
		public string DaqRevision
		{
			get
			{
				return this._daqVersion;
			}
			set
			{
				if (value.Length > 20)
				{
					this._daqVersion = value.Substring(0, 20);
					return;
				}
				this._daqVersion = value;
			}
		}

		// Token: 0x0400002D RID: 45
		private const string FileName = "NF\\UnitInfo.xml";

		// Token: 0x0400002E RID: 46
		public static UnitInfo Instance;

		// Token: 0x0400002F RID: 47
		private string _unitName;

		// Token: 0x04000030 RID: 48
		private string _unitSerialNumber;

		// Token: 0x04000031 RID: 49
		private string _daqSerialNumber;

		// Token: 0x04000032 RID: 50
		private string _hpicSerialNumber;

		// Token: 0x04000033 RID: 51
		private string _electrometerSerialNumber;

		// Token: 0x04000034 RID: 52
		private string _unitVersion;

		// Token: 0x04000035 RID: 53
		private string _daqVersion;
	}
}
