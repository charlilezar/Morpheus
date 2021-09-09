using System;

namespace Model.Commands
{
	// Token: 0x0200000E RID: 14
	public class GetPerformanceMeasurement
	{
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600003C RID: 60 RVA: 0x000022DA File Offset: 0x000004DA
		// (set) Token: 0x0600003B RID: 59 RVA: 0x000022D1 File Offset: 0x000004D1
		public string Name { get; set; }

		// Token: 0x0600003D RID: 61 RVA: 0x000022E2 File Offset: 0x000004E2
		public GetPerformanceMeasurement()
		{
		}

		// Token: 0x0600003E RID: 62 RVA: 0x000022EA File Offset: 0x000004EA
		public GetPerformanceMeasurement(string name)
		{
			this.Name = name;
		}

		// Token: 0x0400001C RID: 28
		public const string XmlElementName = "GetPerformanceMeasurement";
	}
}
