using System;

namespace Model.Commands
{
	// Token: 0x02000035 RID: 53
	public class GetAllEvents
	{
		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000150 RID: 336 RVA: 0x00003483 File Offset: 0x00001683
		// (set) Token: 0x06000151 RID: 337 RVA: 0x0000348B File Offset: 0x0000168B
		public bool UpdatesOnly { get; set; }

		// Token: 0x06000152 RID: 338 RVA: 0x00003494 File Offset: 0x00001694
		public GetAllEvents()
		{
			this.UpdatesOnly = false;
		}

		// Token: 0x0400009D RID: 157
		public const string XmlElementName = "GetAllEvents";
	}
}
