using System;

namespace Model.Commands
{
	// Token: 0x02000046 RID: 70
	public class GetTimeResponse
	{
		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000193 RID: 403 RVA: 0x00003779 File Offset: 0x00001979
		// (set) Token: 0x06000194 RID: 404 RVA: 0x00003781 File Offset: 0x00001981
		public DateTime Time { get; set; }

		// Token: 0x040000C3 RID: 195
		public const string XmlElementName = "GetTimeResponse";
	}
}
