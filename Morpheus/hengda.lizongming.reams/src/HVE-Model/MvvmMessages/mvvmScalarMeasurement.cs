using System;
using Model.Commands;
using Model.DataService;
using Model.Measurements;

namespace Model.MvvmMessages
{
	// Token: 0x02000097 RID: 151
	public class mvvmScalarMeasurement : mvvmMessage
	{
		// Token: 0x17000192 RID: 402
		// (get) Token: 0x06000494 RID: 1172 RVA: 0x0000B31E File Offset: 0x0000951E
		// (set) Token: 0x06000495 RID: 1173 RVA: 0x0000B326 File Offset: 0x00009526
		public ScalarMeasurement Measurement { get; private set; }

		// Token: 0x06000496 RID: 1174 RVA: 0x0000B32F File Offset: 0x0000952F
		public mvvmScalarMeasurement(SentryDataService dataService, ScalarMeasurement meas) : base(dataService)
		{
			this.Measurement = meas;
		}

		// Token: 0x06000497 RID: 1175 RVA: 0x0000B33F File Offset: 0x0000953F
		public mvvmScalarMeasurement(SentryDataService dataService, ScalarMeasurementResponse resp) : base(dataService)
		{
			this.Measurement = resp.Measurement;
		}
	}
}
