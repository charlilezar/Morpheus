using System;
using Model.Measurements;

namespace Model.Commands
{
	// Token: 0x0200000C RID: 12
	public class BatteryMeasurementResponse
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600002B RID: 43 RVA: 0x00002225 File Offset: 0x00000425
		// (set) Token: 0x0600002C RID: 44 RVA: 0x0000222D File Offset: 0x0000042D
		public string Name { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002236 File Offset: 0x00000436
		// (set) Token: 0x0600002E RID: 46 RVA: 0x0000223E File Offset: 0x0000043E
		public DateTime Time { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00002247 File Offset: 0x00000447
		// (set) Token: 0x06000030 RID: 48 RVA: 0x0000224F File Offset: 0x0000044F
		public BatteryMeasurement Measurement { get; set; }

		// Token: 0x06000031 RID: 49 RVA: 0x00002258 File Offset: 0x00000458
		public BatteryMeasurementResponse()
		{
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002260 File Offset: 0x00000460
		public BatteryMeasurementResponse(BatteryMeasurement measurement)
		{
			this.Measurement = measurement;
		}
	}
}
