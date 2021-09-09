using System;
using Model.Commands;
using Model.DataService;
using Model.Measurements;

namespace Model.MvvmMessages
{
	// Token: 0x0200009B RID: 155
	public class mvvmPerformanceMeasurementResponse : mvvmMessage
	{
		// Token: 0x17000196 RID: 406
		// (get) Token: 0x060004A2 RID: 1186 RVA: 0x0000B3CC File Offset: 0x000095CC
		// (set) Token: 0x060004A3 RID: 1187 RVA: 0x0000B3D4 File Offset: 0x000095D4
		public PerformanceMeasurement Measurement { get; private set; }

		// Token: 0x060004A4 RID: 1188 RVA: 0x0000B3DD File Offset: 0x000095DD
		public mvvmPerformanceMeasurementResponse(SentryDataService dataService, PerformanceMeasurementResponse resp) : base(dataService)
		{
			this.Measurement = resp.Measurement;
		}
	}
}
