using System;
using System.Xml.Serialization;
using Model.Configurations;

namespace Model.Measurements
{
	// Token: 0x02000084 RID: 132
	public abstract class CompositeMeasurement : Measurement
	{
		// Token: 0x17000115 RID: 277
		// (get) Token: 0x060003A3 RID: 931 RVA: 0x00009B83 File Offset: 0x00007D83
		// (set) Token: 0x060003A4 RID: 932 RVA: 0x00009B8B File Offset: 0x00007D8B
		public CompositeMeasurementConfiguration Configuration { get; set; }

		// Token: 0x060003A5 RID: 933 RVA: 0x00009B94 File Offset: 0x00007D94
		protected CompositeMeasurement()
		{
			this.Configuration = new CompositeMeasurementConfiguration();
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x00009BA7 File Offset: 0x00007DA7
		protected CompositeMeasurement(CompositeMeasurementConfiguration config) : base(config.Name)
		{
			this.Configuration = config;
		}

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x060003A7 RID: 935 RVA: 0x00009BBC File Offset: 0x00007DBC
		// (set) Token: 0x060003A8 RID: 936 RVA: 0x00009BC4 File Offset: 0x00007DC4
		public string CompositeType { get; set; }

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x060003A9 RID: 937
		// (set) Token: 0x060003AA RID: 938
		[XmlIgnore]
		public abstract string CompositeValue { get; set; }

		// Token: 0x040002BE RID: 702
		public static WaveformMeasurement ElectrometerX1Waveform;

		// Token: 0x040002BF RID: 703
		public static WaveformMeasurement ElectrometerX10Waveform;

		// Token: 0x040002C0 RID: 704
		public static BatteryMeasurement BatteryStatusMeasurement;
	}
}
