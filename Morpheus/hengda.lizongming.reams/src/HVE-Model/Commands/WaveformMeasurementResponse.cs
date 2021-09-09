using System;
using Model.Measurements;

namespace Model.Commands
{
	// Token: 0x0200000B RID: 11
	public class WaveformMeasurementResponse
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000023 RID: 35 RVA: 0x000021DB File Offset: 0x000003DB
		// (set) Token: 0x06000024 RID: 36 RVA: 0x000021E3 File Offset: 0x000003E3
		public string Name { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000025 RID: 37 RVA: 0x000021EC File Offset: 0x000003EC
		// (set) Token: 0x06000026 RID: 38 RVA: 0x000021F4 File Offset: 0x000003F4
		public DateTime Time { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000027 RID: 39 RVA: 0x000021FD File Offset: 0x000003FD
		// (set) Token: 0x06000028 RID: 40 RVA: 0x00002205 File Offset: 0x00000405
		public WaveformMeasurement Measurement { get; set; }

		// Token: 0x06000029 RID: 41 RVA: 0x0000220E File Offset: 0x0000040E
		public WaveformMeasurementResponse()
		{
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002216 File Offset: 0x00000416
		public WaveformMeasurementResponse(WaveformMeasurement measurement)
		{
			this.Measurement = measurement;
		}
	}
}
