using System;

namespace Model.Commands
{
	// Token: 0x02000043 RID: 67
	public class LogTestMeasurements
	{
		// Token: 0x1700007B RID: 123
		// (get) Token: 0x0600017D RID: 381 RVA: 0x0000365C File Offset: 0x0000185C
		// (set) Token: 0x0600017C RID: 380 RVA: 0x00003653 File Offset: 0x00001853
		public int ConfigId { get; set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x0600017F RID: 383 RVA: 0x0000366D File Offset: 0x0000186D
		// (set) Token: 0x0600017E RID: 382 RVA: 0x00003664 File Offset: 0x00001864
		public int Count { get; set; }

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000181 RID: 385 RVA: 0x0000367E File Offset: 0x0000187E
		// (set) Token: 0x06000180 RID: 384 RVA: 0x00003675 File Offset: 0x00001875
		public int Delay { get; set; }

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000183 RID: 387 RVA: 0x0000368F File Offset: 0x0000188F
		// (set) Token: 0x06000182 RID: 386 RVA: 0x00003686 File Offset: 0x00001886
		public bool DelayBetweenWrites { get; set; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000185 RID: 389 RVA: 0x000036A0 File Offset: 0x000018A0
		// (set) Token: 0x06000184 RID: 388 RVA: 0x00003697 File Offset: 0x00001897
		public int IntervalSeconds { get; set; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000187 RID: 391 RVA: 0x000036B1 File Offset: 0x000018B1
		// (set) Token: 0x06000186 RID: 390 RVA: 0x000036A8 File Offset: 0x000018A8
		public bool UseMeasurementLoggingInterval { get; set; }

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000189 RID: 393 RVA: 0x000036C2 File Offset: 0x000018C2
		// (set) Token: 0x06000188 RID: 392 RVA: 0x000036B9 File Offset: 0x000018B9
		public bool WriteForward { get; set; }

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600018B RID: 395 RVA: 0x000036D3 File Offset: 0x000018D3
		// (set) Token: 0x0600018A RID: 394 RVA: 0x000036CA File Offset: 0x000018CA
		public DateTime RecordingDate { get; set; }

		// Token: 0x0600018C RID: 396 RVA: 0x000036DB File Offset: 0x000018DB
		public LogTestMeasurements()
		{
		}

		// Token: 0x0600018D RID: 397 RVA: 0x000036E4 File Offset: 0x000018E4
		public LogTestMeasurements(int configId, int count, int intervalSeconds, bool delayBetweenWrites, int delay, bool writeForward, DateTime recordingDate, bool useMeasurementLoggingInterval)
		{
			this.ConfigId = configId;
			this.DelayBetweenWrites = delayBetweenWrites;
			this.Count = count;
			this.IntervalSeconds = intervalSeconds;
			this.Delay = delay;
			this.RecordingDate = recordingDate;
			this.WriteForward = writeForward;
			this.UseMeasurementLoggingInterval = useMeasurementLoggingInterval;
		}

		// Token: 0x040000B7 RID: 183
		public const string XmlElementName = "LogTestMeasurements";
	}
}
