using System;

namespace Model.Measurements
{
	// Token: 0x02000086 RID: 134
	public class MeteorologicalData
	{
		// Token: 0x060003B4 RID: 948 RVA: 0x00009D34 File Offset: 0x00007F34
		public MeteorologicalData()
		{
			this.Instance = this;
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x060003B5 RID: 949 RVA: 0x00009D43 File Offset: 0x00007F43
		// (set) Token: 0x060003B6 RID: 950 RVA: 0x00009D4B File Offset: 0x00007F4B
		public double WindSpeedAverage { get; set; }

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x060003B7 RID: 951 RVA: 0x00009D54 File Offset: 0x00007F54
		// (set) Token: 0x060003B8 RID: 952 RVA: 0x00009D5C File Offset: 0x00007F5C
		public double WindSpeedMinimum { get; set; }

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x060003B9 RID: 953 RVA: 0x00009D65 File Offset: 0x00007F65
		// (set) Token: 0x060003BA RID: 954 RVA: 0x00009D6D File Offset: 0x00007F6D
		public double WindSpeedMaximum { get; set; }

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x060003BB RID: 955 RVA: 0x00009D76 File Offset: 0x00007F76
		// (set) Token: 0x060003BC RID: 956 RVA: 0x00009D7E File Offset: 0x00007F7E
		public double WindDirectionAverage { get; set; }

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x060003BD RID: 957 RVA: 0x00009D87 File Offset: 0x00007F87
		// (set) Token: 0x060003BE RID: 958 RVA: 0x00009D8F File Offset: 0x00007F8F
		public double WindDirectionMinimum { get; set; }

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x060003BF RID: 959 RVA: 0x00009D98 File Offset: 0x00007F98
		// (set) Token: 0x060003C0 RID: 960 RVA: 0x00009DA0 File Offset: 0x00007FA0
		public double WindDirectionMaximum { get; set; }

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x060003C1 RID: 961 RVA: 0x00009DA9 File Offset: 0x00007FA9
		// (set) Token: 0x060003C2 RID: 962 RVA: 0x00009DB1 File Offset: 0x00007FB1
		public double AirTemperature { get; set; }

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x060003C3 RID: 963 RVA: 0x00009DBA File Offset: 0x00007FBA
		// (set) Token: 0x060003C4 RID: 964 RVA: 0x00009DC2 File Offset: 0x00007FC2
		public double RelativeHumidty { get; set; }

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x060003C5 RID: 965 RVA: 0x00009DCB File Offset: 0x00007FCB
		// (set) Token: 0x060003C6 RID: 966 RVA: 0x00009DD3 File Offset: 0x00007FD3
		public double AirPressure { get; set; }

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x060003C7 RID: 967 RVA: 0x00009DDC File Offset: 0x00007FDC
		// (set) Token: 0x060003C8 RID: 968 RVA: 0x00009DE4 File Offset: 0x00007FE4
		public double RainAccumulation { get; set; }

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x060003C9 RID: 969 RVA: 0x00009DED File Offset: 0x00007FED
		// (set) Token: 0x060003CA RID: 970 RVA: 0x00009DF5 File Offset: 0x00007FF5
		public double RainDuration { get; set; }

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x060003CB RID: 971 RVA: 0x00009DFE File Offset: 0x00007FFE
		// (set) Token: 0x060003CC RID: 972 RVA: 0x00009E06 File Offset: 0x00008006
		public double RainIntensity { get; set; }

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x060003CD RID: 973 RVA: 0x00009E0F File Offset: 0x0000800F
		// (set) Token: 0x060003CE RID: 974 RVA: 0x00009E17 File Offset: 0x00008017
		public double RainPeakIntensity { get; set; }

		// Token: 0x040002C5 RID: 709
		public MeteorologicalData Instance;
	}
}
