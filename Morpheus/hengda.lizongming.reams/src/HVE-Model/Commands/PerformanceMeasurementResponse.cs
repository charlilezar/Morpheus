using System;
using Model.Measurements;

namespace Model.Commands
{
	// Token: 0x0200000D RID: 13
	public class PerformanceMeasurementResponse
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000033 RID: 51 RVA: 0x0000226F File Offset: 0x0000046F
		// (set) Token: 0x06000034 RID: 52 RVA: 0x00002277 File Offset: 0x00000477
		public string Name { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00002280 File Offset: 0x00000480
		// (set) Token: 0x06000036 RID: 54 RVA: 0x00002288 File Offset: 0x00000488
		public DateTime Time { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00002291 File Offset: 0x00000491
		// (set) Token: 0x06000038 RID: 56 RVA: 0x00002299 File Offset: 0x00000499
		public PerformanceMeasurement Measurement { get; set; }

		// Token: 0x06000039 RID: 57 RVA: 0x000022A2 File Offset: 0x000004A2
		public PerformanceMeasurementResponse()
		{
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000022AA File Offset: 0x000004AA
		public PerformanceMeasurementResponse(PerformanceMeasurement measurement)
		{
			this.Name = measurement.Name;
			this.Time = measurement.Time;
			this.Measurement = measurement;
		}

		// Token: 0x04000018 RID: 24
		public const string XmlElementName = "PerformanceMeasurementResponse";
	}
}
