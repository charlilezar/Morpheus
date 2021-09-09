using System;

namespace Model.Commands
{
	// Token: 0x02000034 RID: 52
	public class WaveformTrigger
	{
		// Token: 0x1700006E RID: 110
		// (get) Token: 0x0600014D RID: 333 RVA: 0x00003464 File Offset: 0x00001664
		// (set) Token: 0x0600014C RID: 332 RVA: 0x0000345B File Offset: 0x0000165B
		public WaveformTriggerMode TriggerMode { get; set; }

		// Token: 0x0600014E RID: 334 RVA: 0x0000346C File Offset: 0x0000166C
		public WaveformTrigger()
		{
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00003474 File Offset: 0x00001674
		public WaveformTrigger(WaveformTriggerMode triggerMode)
		{
			this.TriggerMode = triggerMode;
		}

		// Token: 0x0400009B RID: 155
		public const string XmlElementName = "WaveFormTrigger";
	}
}
