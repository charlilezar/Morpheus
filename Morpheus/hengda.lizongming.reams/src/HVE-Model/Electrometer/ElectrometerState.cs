using System;

namespace Model.Electrometer
{
	// Token: 0x0200007F RID: 127
	public static class ElectrometerState
	{
		// Token: 0x17000108 RID: 264
		// (get) Token: 0x0600037D RID: 893 RVA: 0x0000972B File Offset: 0x0000792B
		// (set) Token: 0x0600037E RID: 894 RVA: 0x00009732 File Offset: 0x00007932
		public static string SerialNumber { get; set; }

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x0600037F RID: 895 RVA: 0x0000973A File Offset: 0x0000793A
		// (set) Token: 0x06000380 RID: 896 RVA: 0x00009741 File Offset: 0x00007941
		public static string Revision { get; set; }

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x06000381 RID: 897 RVA: 0x00009749 File Offset: 0x00007949
		// (set) Token: 0x06000382 RID: 898 RVA: 0x00009750 File Offset: 0x00007950
		public static ElectrometerConfiguration Configuration { get; set; }

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000383 RID: 899 RVA: 0x00009758 File Offset: 0x00007958
		public static int ADSampleInterval
		{
			get
			{
				return (int)(5.0 * ElectrometerState.Configuration.AcquisitionTime);
			}
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000384 RID: 900 RVA: 0x00009770 File Offset: 0x00007970
		public static double LeakageCurrent
		{
			get
			{
				int temperature = 25;
				if (ElectrometerState.GetElectrometerTemperature != null)
				{
					temperature = (int)ElectrometerState.GetElectrometerTemperature();
				}
				return ElectrometerState.Configuration.LeakageBreakpoints.GetValue(temperature) * 1E-15;
			}
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000385 RID: 901 RVA: 0x000097B0 File Offset: 0x000079B0
		public static double LowRangeCapacitance
		{
			get
			{
				int temperature = 25;
				if (ElectrometerState.GetElectrometerTemperature != null)
				{
					temperature = (int)ElectrometerState.GetElectrometerTemperature();
				}
				return ElectrometerState.Configuration.LowRangeCapacitorBreakpoints.GetValue(temperature) * 1E-12;
			}
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x06000386 RID: 902 RVA: 0x000097F0 File Offset: 0x000079F0
		public static double HighRangeCapacitance
		{
			get
			{
				int temperature = 25;
				if (ElectrometerState.GetElectrometerTemperature != null)
				{
					temperature = (int)ElectrometerState.GetElectrometerTemperature();
				}
				return ElectrometerState.Configuration.HighRangeCapacitorBreakpoints.GetValue(temperature) * 1E-12;
			}
		}

		// Token: 0x04000270 RID: 624
		private static readonly Func<double> GetElectrometerTemperature;
	}
}
