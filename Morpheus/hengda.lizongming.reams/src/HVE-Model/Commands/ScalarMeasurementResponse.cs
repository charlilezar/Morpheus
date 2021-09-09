using System;
using Model.Measurements;

namespace Model.Commands
{
	// Token: 0x02000042 RID: 66
	public class ScalarMeasurementResponse
	{
		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000178 RID: 376 RVA: 0x0000362B File Offset: 0x0000182B
		// (set) Token: 0x06000179 RID: 377 RVA: 0x00003633 File Offset: 0x00001833
		public ScalarMeasurement Measurement { get; set; }

		// Token: 0x0600017A RID: 378 RVA: 0x0000363C File Offset: 0x0000183C
		public ScalarMeasurementResponse()
		{
		}

		// Token: 0x0600017B RID: 379 RVA: 0x00003644 File Offset: 0x00001844
		public ScalarMeasurementResponse(ScalarMeasurement measurement)
		{
			this.Measurement = measurement;
		}
	}
}
