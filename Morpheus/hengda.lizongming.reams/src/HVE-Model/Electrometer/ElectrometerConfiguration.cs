using System;

namespace Model.Electrometer
{
	// Token: 0x0200007E RID: 126
	public class ElectrometerConfiguration
	{
		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x06000358 RID: 856 RVA: 0x000093A9 File Offset: 0x000075A9
		// (set) Token: 0x06000359 RID: 857 RVA: 0x000093B1 File Offset: 0x000075B1
		public TemperatureSplineFit LeakageBreakpoints { get; set; }

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x0600035A RID: 858 RVA: 0x000093BA File Offset: 0x000075BA
		// (set) Token: 0x0600035B RID: 859 RVA: 0x000093C2 File Offset: 0x000075C2
		public TemperatureSplineFit LowRangeCapacitorBreakpoints { get; set; }

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x0600035C RID: 860 RVA: 0x000093CB File Offset: 0x000075CB
		// (set) Token: 0x0600035D RID: 861 RVA: 0x000093D3 File Offset: 0x000075D3
		public TemperatureSplineFit HighRangeCapacitorBreakpoints { get; set; }

		// Token: 0x0600035E RID: 862 RVA: 0x000093DC File Offset: 0x000075DC
		public ElectrometerConfiguration()
		{
			ElectrometerConfiguration._instance = this;
			this.AcquisitionTime = 4.5;
			this.ResetHoldTime = 0.01;
			this.PostIntegratorReleaseTimeLowRange = 0.05;
			this.PostIntegratorReleaseTimeHighRange = 0.05;
			this.X10Gain = 10.0;
			this.ADFullScale = 4.5;
			this.MinIntegrationVoltage = 0.2;
			this.HighToLowX10SlopeThreshold = -0.003;
			this.LowToHighX1SlopeThreshold = -10.0;
			this.MinVoltageThreshold = 0.2;
			this.GainCrossoverFactor = 0.24;
		}

		// Token: 0x0600035F RID: 863 RVA: 0x0000949A File Offset: 0x0000769A
		public ElectrometerConfiguration(bool dummyNonSerializeConstructor) : this()
		{
			this.LeakageBreakpoints = new TemperatureSplineFit("Leakage");
			this.LowRangeCapacitorBreakpoints = new TemperatureSplineFit("Low Range Capacitor");
			this.HighRangeCapacitorBreakpoints = new TemperatureSplineFit("High Range Capacitor");
			this.SetupDefaultBreakpoints();
		}

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x06000360 RID: 864 RVA: 0x000094D8 File Offset: 0x000076D8
		public static ElectrometerConfiguration Instance
		{
			get
			{
				return ElectrometerConfiguration._instance;
			}
		}

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x06000361 RID: 865 RVA: 0x000094DF File Offset: 0x000076DF
		// (set) Token: 0x06000362 RID: 866 RVA: 0x000094E7 File Offset: 0x000076E7
		public double AcquisitionTime
		{
			get
			{
				return this._acquisitionTime;
			}
			set
			{
				this._acquisitionTime = ((value > 20.0) ? 20.0 : value);
			}
		}

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000363 RID: 867 RVA: 0x00009507 File Offset: 0x00007707
		// (set) Token: 0x06000364 RID: 868 RVA: 0x0000950F File Offset: 0x0000770F
		public double ResetHoldTime { get; set; }

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000365 RID: 869 RVA: 0x00009518 File Offset: 0x00007718
		// (set) Token: 0x06000366 RID: 870 RVA: 0x00009520 File Offset: 0x00007720
		public double PostIntegratorReleaseTimeLowRange { get; set; }

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x06000367 RID: 871 RVA: 0x00009529 File Offset: 0x00007729
		// (set) Token: 0x06000368 RID: 872 RVA: 0x00009531 File Offset: 0x00007731
		public double PostIntegratorReleaseTimeHighRange { get; set; }

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06000369 RID: 873 RVA: 0x0000953A File Offset: 0x0000773A
		// (set) Token: 0x0600036A RID: 874 RVA: 0x00009542 File Offset: 0x00007742
		public double X10Gain { get; set; }

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x0600036B RID: 875 RVA: 0x0000954B File Offset: 0x0000774B
		// (set) Token: 0x0600036C RID: 876 RVA: 0x00009553 File Offset: 0x00007753
		public double ADFullScale { get; set; }

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x0600036D RID: 877 RVA: 0x0000955C File Offset: 0x0000775C
		// (set) Token: 0x0600036E RID: 878 RVA: 0x00009564 File Offset: 0x00007764
		public double MinIntegrationVoltage { get; set; }

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x0600036F RID: 879 RVA: 0x0000956D File Offset: 0x0000776D
		// (set) Token: 0x06000370 RID: 880 RVA: 0x00009575 File Offset: 0x00007775
		public double HighToLowX10SlopeThreshold { get; set; }

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x06000371 RID: 881 RVA: 0x0000957E File Offset: 0x0000777E
		// (set) Token: 0x06000372 RID: 882 RVA: 0x00009586 File Offset: 0x00007786
		public double LowToHighX1SlopeThreshold { get; set; }

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x06000373 RID: 883 RVA: 0x0000958F File Offset: 0x0000778F
		// (set) Token: 0x06000374 RID: 884 RVA: 0x00009597 File Offset: 0x00007797
		public double MinVoltageThreshold { get; set; }

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x06000375 RID: 885 RVA: 0x000095A0 File Offset: 0x000077A0
		// (set) Token: 0x06000376 RID: 886 RVA: 0x000095A8 File Offset: 0x000077A8
		public double GainCrossoverFactor { get; set; }

		// Token: 0x06000377 RID: 887 RVA: 0x000095B1 File Offset: 0x000077B1
		public static void CalculateTemperatureRelateValues()
		{
			ElectrometerConfiguration.Instance.LeakageBreakpoints.CalculateTemperatureRelateValues();
			ElectrometerConfiguration.Instance.LowRangeCapacitorBreakpoints.CalculateTemperatureRelateValues();
			ElectrometerConfiguration.Instance.HighRangeCapacitorBreakpoints.CalculateTemperatureRelateValues();
		}

		// Token: 0x06000378 RID: 888 RVA: 0x000095E0 File Offset: 0x000077E0
		public void ClearLeakageBreakpoints()
		{
			this.LeakageBreakpoints.Clear();
		}

		// Token: 0x06000379 RID: 889 RVA: 0x000095ED File Offset: 0x000077ED
		public void ClearLowRangeCapacitorBreakpoints()
		{
			this.LowRangeCapacitorBreakpoints.Clear();
		}

		// Token: 0x0600037A RID: 890 RVA: 0x000095FA File Offset: 0x000077FA
		public void ClearHighRangeCapacitorBreakpoints()
		{
			this.HighRangeCapacitorBreakpoints.Clear();
		}

		// Token: 0x0600037B RID: 891 RVA: 0x00009608 File Offset: 0x00007808
		public bool AreBreakpointsValid()
		{
			return this.LeakageBreakpoints.Breakpoints.Count >= 2 && this.LowRangeCapacitorBreakpoints.Breakpoints.Count >= 2 && this.HighRangeCapacitorBreakpoints.Breakpoints.Count >= 2;
		}

		// Token: 0x0600037C RID: 892 RVA: 0x00009658 File Offset: 0x00007858
		public void SetupDefaultBreakpoints()
		{
			this.LeakageBreakpoints.Clear();
			this.LowRangeCapacitorBreakpoints.Clear();
			this.HighRangeCapacitorBreakpoints.Clear();
			this.LeakageBreakpoints.AddBreakpoint(-40, 0.0);
			this.LeakageBreakpoints.AddBreakpoint(55, 0.0);
			this.LeakageBreakpoints.CalculateTemperatureRelateValues();
			this.LowRangeCapacitorBreakpoints.AddBreakpoint(-40, 4.7);
			this.LowRangeCapacitorBreakpoints.AddBreakpoint(55, 4.7);
			this.LowRangeCapacitorBreakpoints.CalculateTemperatureRelateValues();
			this.HighRangeCapacitorBreakpoints.AddBreakpoint(-40, 69000.0);
			this.HighRangeCapacitorBreakpoints.AddBreakpoint(55, 69000.0);
			this.HighRangeCapacitorBreakpoints.CalculateTemperatureRelateValues();
		}

		// Token: 0x0400025F RID: 607
		public const int MaxAcquisitionTime = 20;

		// Token: 0x04000260 RID: 608
		public const string XmlElementName = "Configuration";

		// Token: 0x04000261 RID: 609
		private static ElectrometerConfiguration _instance;

		// Token: 0x04000262 RID: 610
		private double _acquisitionTime;
	}
}
