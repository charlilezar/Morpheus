using System;
using Model.DataService;
using Model.Measurements;

namespace Model.MvvmMessages
{
	// Token: 0x02000099 RID: 153
	public class mvvmWaveformMeasurement : mvvmMessage
	{
		// Token: 0x17000194 RID: 404
		// (get) Token: 0x0600049C RID: 1180 RVA: 0x0000B38A File Offset: 0x0000958A
		// (set) Token: 0x0600049D RID: 1181 RVA: 0x0000B392 File Offset: 0x00009592
		public WaveformMeasurement Measurement { get; private set; }

		// Token: 0x0600049E RID: 1182 RVA: 0x0000B39B File Offset: 0x0000959B
		public mvvmWaveformMeasurement(SentryDataService dataService, WaveformMeasurement meas) : base(dataService)
		{
			this.Measurement = meas;
		}
	}
}
