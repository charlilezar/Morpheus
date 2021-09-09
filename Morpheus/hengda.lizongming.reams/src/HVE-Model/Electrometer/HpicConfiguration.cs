using System;

namespace Model.Electrometer
{
	// Token: 0x02000080 RID: 128
	public class HpicConfiguration
	{
		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000388 RID: 904 RVA: 0x0000982F File Offset: 0x00007A2F
		// (set) Token: 0x06000389 RID: 905 RVA: 0x00009837 File Offset: 0x00007A37
		public double Sensitivity { get; set; }

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x0600038B RID: 907 RVA: 0x00009863 File Offset: 0x00007A63
		// (set) Token: 0x0600038A RID: 906 RVA: 0x00009840 File Offset: 0x00007A40
		public string SerialNumber
		{
			get
			{
				return this._serialNumber;
			}
			set
			{
				if (value.Length > 20)
				{
					this._serialNumber = value.Substring(0, 20);
					return;
				}
				this._serialNumber = value;
			}
		}

		// Token: 0x0600038C RID: 908 RVA: 0x0000986B File Offset: 0x00007A6B
		public HpicConfiguration()
		{
			HpicConfiguration.Instance = this;
		}

		// Token: 0x0600038D RID: 909 RVA: 0x00009879 File Offset: 0x00007A79
		public HpicConfiguration(string serialNumber, double hpicSensitivity)
		{
			HpicConfiguration.Instance = this;
			this.Sensitivity = hpicSensitivity;
			this.SerialNumber = serialNumber;
		}

		// Token: 0x04000274 RID: 628
		public const double DefaultHpicSensitivity = 2.2E-08;

		// Token: 0x04000275 RID: 629
		public static HpicConfiguration Instance;

		// Token: 0x04000276 RID: 630
		private string _serialNumber;
	}
}
