using System;

namespace Model.Commands
{
	// Token: 0x0200001D RID: 29
	public class GetDatabaseMeasurementSummary
	{
		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x00002882 File Offset: 0x00000A82
		// (set) Token: 0x060000A8 RID: 168 RVA: 0x0000288A File Offset: 0x00000A8A
		public DateTime StartTime { get; set; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x00002893 File Offset: 0x00000A93
		// (set) Token: 0x060000AA RID: 170 RVA: 0x0000289B File Offset: 0x00000A9B
		public DateTime EndTime { get; set; }

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000AB RID: 171 RVA: 0x000028A4 File Offset: 0x00000AA4
		// (set) Token: 0x060000AC RID: 172 RVA: 0x000028AC File Offset: 0x00000AAC
		public string MeasurementName { get; set; }

		// Token: 0x060000AD RID: 173 RVA: 0x000028B5 File Offset: 0x00000AB5
		public GetDatabaseMeasurementSummary()
		{
		}

		// Token: 0x060000AE RID: 174 RVA: 0x000028BD File Offset: 0x00000ABD
		public GetDatabaseMeasurementSummary(string measurementName, DateTime startTime, DateTime endTime)
		{
			this.StartTime = startTime;
			this.EndTime = endTime;
			this.MeasurementName = measurementName;
		}

		// Token: 0x0400004F RID: 79
		public const string XmlElementName = "GetDatabaseMeasurementSummary";
	}
}
