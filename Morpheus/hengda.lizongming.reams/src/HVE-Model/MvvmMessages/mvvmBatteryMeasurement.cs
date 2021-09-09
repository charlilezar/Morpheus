using System;
using Model.DataService;
using Model.Measurements;

namespace Model.MvvmMessages
{
	// Token: 0x0200009A RID: 154
	public class mvvmBatteryMeasurement : mvvmMessage
	{
		// Token: 0x17000195 RID: 405
		// (get) Token: 0x0600049F RID: 1183 RVA: 0x0000B3AB File Offset: 0x000095AB
		// (set) Token: 0x060004A0 RID: 1184 RVA: 0x0000B3B3 File Offset: 0x000095B3
		public BatteryMeasurement Measurement { get; private set; }

		// Token: 0x060004A1 RID: 1185 RVA: 0x0000B3BC File Offset: 0x000095BC
		public mvvmBatteryMeasurement(SentryDataService dataService, BatteryMeasurement meas) : base(dataService)
		{
			this.Measurement = meas;
		}
	}
}
